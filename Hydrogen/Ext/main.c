#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <stdint.h>
#include <string.h>
#include <stdlib.h>
#include <windows.h>

#define RESPONSE_TIMEOUT_MS 15000
static HANDLE serial_handle;

static int serial_write_all(const void* data, size_t length) {
    const unsigned char* p = data;
    while (length) {
        DWORD n = 0, want = length > 4096 ? 4096 : (DWORD)length;
        if (!WriteFile(serial_handle, p, want, &n, NULL) || !n) {
            return 0;
        }
        p += n;
        length -= n;
    }
    return 1;
}

static int serial_read_exact(void* data, size_t length) {
    unsigned char* p = data;
    ULONGLONG deadline_ms = GetTickCount64() + RESPONSE_TIMEOUT_MS;
    while (length && GetTickCount64() < deadline_ms) {
        DWORD n = 0, want = length > 4096 ? 4096 : (DWORD)length;
        if (!ReadFile(serial_handle, p, want, &n, NULL)) {
            return 0;
        }
        p += n;
        length -= n;
    }
    return length == 0;
}

static int receive_response_line(char* response_text, size_t capacity) {
    size_t n = 0;
    unsigned char c;
    ULONGLONG deadline_ms = GetTickCount64() + RESPONSE_TIMEOUT_MS;
    while (GetTickCount64() < deadline_ms) {
        DWORD got = 0;
        if (!ReadFile(serial_handle, &c, 1, &got, NULL)) {
            return 0;
        }
        if (!got) {
            continue;
        }
        if (c == '\n') {
            response_text[n] = 0;
            return 1;
        }
        if (c == '\r') {
            continue;
        }
        if (!c || n + 1 >= capacity) {
            return 0;
        }
        response_text[n++] = (char)c;
    }
    return 0;
}

static int expect_response(const char* expected) {
    char response_text[256];
    if (!receive_response_line(response_text, sizeof(response_text))) {
        puts("Response timeout/error.");
        return 0;
    }
    if (strcmp(response_text, expected)) {
        printf("Expected [%s], received [%s]\n", expected, response_text);
        return 0;
    }
    return 1;
}

static int send_command_line(const char* response_text) {
    return serial_write_all(response_text, strlen(response_text)) && serial_write_all("\n", 1);
}

static int read_console_line(const char* prompt, char* response_text, size_t capacity) {
    fputs(prompt, stdout);
    fflush(stdout);
    if (!fgets(response_text, (int)capacity, stdin)) {
        return 0;
    }
    if (!strchr(response_text, '\n') && !feof(stdin)) {
        int c;
        while ((c = getchar()) != '\n' && c != EOF) {
        }
        puts("Input too long.");
        return 0;
    }
    response_text[strcspn(response_text, "\r\n")] = 0;
    return 1;
}

static int parse_response_number(const char* response_text, const char* prefix, uint32_t* result) {
    size_t n = strlen(prefix);
    if (strncmp(response_text, prefix, n)) {
        return 0;
    }
    response_text += n;
    if (!*response_text) {
        return 0;
    }
    uint32_t value = 0;
    for (; *response_text; ++response_text) {
        if (*response_text < '0' || *response_text > '9' ||
            value > (INT32_MAX - (unsigned)(*response_text - '0')) / 10U) {
            return 0;
        }
        value = value * 10U + (unsigned)(*response_text - '0');
    }
    *result = value;
    return 1;
}

/* 직접 입력한 텍스트와 로컬 파일을 동일한 64바이트 프로토콜로 보냅니다. */
static int upload_file_or_text(void) {
    char text[4096], response_text[128];
    FILE* local_file = NULL;
    uint32_t total_bytes, sent_bytes = 0;

    /* Board has responded SIZE? and waits indefinitely for user choice. */
    if (!read_console_line("Text, or p\"C:\\path\\file.txt\": ", text, sizeof(text))) {
        return send_command_line("CANCEL") && expect_response("CANCELLED");
    }
    if (!strncmp(text, "p\"", 2)) {
        size_t n = strlen(text);
        if (n < 4 || text[n - 1] != '"') {
            puts("Invalid quoted path.");
            return send_command_line("CANCEL") && expect_response("CANCELLED");
        }
        text[n - 1] = 0;
        local_file = fopen(text + 2, "rb");
        if (!local_file) {
            perror("fopen");
            return send_command_line("CANCEL") && expect_response("CANCELLED");
        }
        if (_fseeki64(local_file, 0, SEEK_END)) {
            fclose(local_file);
            return send_command_line("CANCEL") && expect_response("CANCELLED");
        }
        __int64 size = _ftelli64(local_file);
        if (size < 0 || size > INT32_MAX || _fseeki64(local_file, 0, SEEK_SET)) {
            puts("Invalid/too large file.");
            fclose(local_file);
            return send_command_line("CANCEL") && expect_response("CANCELLED");
        }
        total_bytes = (uint32_t)size;
    }
    else {
        total_bytes = (uint32_t)strlen(text);
    }

    /* 총 길이를 알리고 보드가 준비될 때까지 기다립니다. */
    snprintf(response_text, sizeof(response_text), "SIZE %lu", (unsigned long)total_bytes);
    if (!send_command_line(response_text) || !expect_response("READY 64")) {
        goto fail;
    }

    /* 마지막 블록은 남은 길이만 전송합니다. */
    while (sent_bytes < total_bytes) {
        unsigned char block[64];
        size_t block_length = total_bytes - sent_bytes;
        if (block_length > 64) {
            block_length = 64;
        }
        if (local_file) {
            if (fread(block, 1, block_length, local_file) != block_length) {
                puts("Local file read failed/changed.");
                goto fail;
            }
        }
        else {
            memcpy(block, text + sent_bytes, block_length);
        }
        if (!serial_write_all(block, block_length)) {
            goto fail;
        }
        sent_bytes += (uint32_t)block_length;
        snprintf(response_text, sizeof(response_text), "ACK %lu", (unsigned long)sent_bytes);

        /* 이번 블록의 ACK를 받은 뒤에만 다음 블록으로 진행합니다. */
        if (!expect_response(response_text)) {
            goto fail; /* No resend: avoids duplicate bytes after lost ACK. */
        }
        printf("\r%lu / %lu bytes", (unsigned long)sent_bytes, (unsigned long)total_bytes);
        fflush(stdout);
    }

    /* ACK와 달리 DONE은 최종 저장 완료를 의미합니다. */
    snprintf(response_text, sizeof(response_text), "DONE %lu", (unsigned long)total_bytes);
    if (!expect_response(response_text)) {
        goto fail;
    }
    if (local_file) {
        fclose(local_file);
    }
    puts("\nSaved successfully.");
    return 1;

fail:
    if (local_file) {
        fclose(local_file);
    }
    puts("\nTransfer stopped. Do not resend in this session. Reset board and reconnect.\n"
        "If DONE was lost, the new file may already be saved; inspect it after reconnecting.");
    return 0;
}

int main(void) {
    char port[64], device[80], command[32], name[64], response_text[256];
    puts("Hydro file editor - 64-byte ACK protocol v1");
    if (!read_console_line("Port (COM3): ", port, sizeof(port))) {
        return 1;
    }
    snprintf(device, sizeof(device), "\\\\.\\%s", port);
    serial_handle =
        CreateFileA(device, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (serial_handle == INVALID_HANDLE_VALUE) {
        printf("Open failed: %lu\n", GetLastError());
        return 1;
    }
    DCB serial_config = { 0 };
    serial_config.DCBlength = sizeof(serial_config);
    if (!GetCommState(serial_handle, &serial_config)) {
        goto error;
    }
    serial_config.BaudRate = CBR_115200;
    serial_config.ByteSize = 8;
    serial_config.StopBits = ONESTOPBIT;
    serial_config.Parity = NOPARITY;
    serial_config.fBinary = TRUE;
    serial_config.fParity = FALSE;
    serial_config.fOutxCtsFlow = FALSE;
    serial_config.fOutxDsrFlow = FALSE;
    serial_config.fDsrSensitivity = FALSE;
    serial_config.fOutX = FALSE;
    serial_config.fInX = FALSE;
    serial_config.fNull = FALSE;
    serial_config.fErrorChar = FALSE;
    serial_config.fAbortOnError = FALSE;
    serial_config.fDtrControl = DTR_CONTROL_DISABLE;
    serial_config.fRtsControl = RTS_CONTROL_DISABLE;
    if (!SetCommState(serial_handle, &serial_config)) {
        goto error;
    }
    COMMTIMEOUTS timeouts = { 0 };
    timeouts.ReadIntervalTimeout = MAXDWORD;
    timeouts.ReadTotalTimeoutConstant = 100;
    timeouts.WriteTotalTimeoutConstant = 3000;
    if (!SetCommTimeouts(serial_handle, &timeouts)) {
        goto error;
    }
    PurgeComm(serial_handle, PURGE_RXCLEAR | PURGE_TXCLEAR);
    const unsigned char start_command[] = { 9, 13, 9, 13, 0xCA, 0xFE, 0x27, 0x22, 0x27, 0x22 };
    if (!serial_write_all(start_command, sizeof(start_command))) {
        goto error;
    }

    /* Before EDITOR READY, ADC binary frames may still be arriving. Scan a marker,
       rather than interpreting binary data as a line or clearing after the request. */
    {
        const char marker[] = "EDITOR READY\n";
        size_t matched_length = 0;
        ULONGLONG deadline_ms = GetTickCount64() + RESPONSE_TIMEOUT_MS;
        while (matched_length < sizeof(marker) - 1 && GetTickCount64() < deadline_ms) {
            unsigned char c;
            DWORD n = 0;
            if (!ReadFile(serial_handle, &c, 1, &n, NULL)) {
                goto error;
            }
            if (n) {
                matched_length = c == (unsigned char)marker[matched_length] ? matched_length + 1
                    : (c == 'E' ? 1 : 0);
            }
        }
        if (matched_length != sizeof(marker) - 1) {
            goto error;
        }
    }
    for (;;) {
        if (!read_console_line("\nCommand (ls/add/mod/del/seek/ext): ", command, sizeof(command))) {
            break;
        }
        if (strcmp(command, "ls") && strcmp(command, "add") && strcmp(command, "mod") &&
            strcmp(command, "del") && strcmp(command, "seek") && strcmp(command, "ext")) {
            puts("Unknown command.");
            continue;
        }
        if (!send_command_line(command)) {
            goto error;
        }
        if (!strcmp(command, "ext")) {
            if (!expect_response("BYE")) {
                goto error;
            }
            break;
        }
        if (!strcmp(command, "ls")) {
            if (!receive_response_line(response_text, sizeof(response_text))) {
                goto error;
            }
            if (strncmp(response_text, "ERR ", 4) == 0) {
                puts(response_text);
                continue;
            }
            if (strcmp(response_text, "LIST")) {
                goto error;
            }
            for (;;) {
                if (!receive_response_line(response_text, sizeof(response_text))) {
                    goto error;
                }
                if (!strcmp(response_text, "END")) {
                    break;
                }
                puts(response_text);
                if (!strncmp(response_text, "ERR ", 4)) {
                    break;
                }
            }
            continue;
        }
        if (!expect_response("NAME")) {
            goto error;
        }
        if (!read_console_line("Board file name: ", name, sizeof(name))) {
            goto error;
        }
        if (!send_command_line(name) ||
            !receive_response_line(response_text, sizeof(response_text))) {
            goto error;
        }
        if (!strncmp(response_text, "ERR ", 4)) {
            puts(response_text);
            continue;
        }
        if (!strcmp(command, "mod")) {
            if (strcmp(response_text, "SIZE?") || !upload_file_or_text()) {
                goto error;
            }
        }
        else if (!strcmp(command, "seek")) {
            uint32_t remaining_bytes;
            if (!parse_response_number(response_text, "DATA ", &remaining_bytes)) {
                goto error;
            }
            while (remaining_bytes) {
                unsigned char block[64];
                size_t n = remaining_bytes > 64 ? 64 : remaining_bytes;
                if (!serial_read_exact(block, n)) {
                    goto error;
                }
                fwrite(block, 1, n, stdout);
                remaining_bytes -= (uint32_t)n;
            }
            puts("");
        }
        else {
            puts(response_text);
        }
    }
    CloseHandle(serial_handle);
    return 0;
error:
    puts("Communication/session failed. Close this program, reset board, then reconnect.");
    CloseHandle(serial_handle);
    return 1;
}
