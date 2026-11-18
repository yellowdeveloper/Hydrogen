// header includes
// standard headers
#include <stdio.h>
#include <string.h>
// win api
#include <windows.h>

#define HEADER1 0x09
#define HEADER2 0x0D
#define HEADER3 0x09
#define HEADER4 0x0D

#define FOOTER1 0x27
#define FOOTER2 0x22
#define FOOTER3 0x27
#define FOOTER4 0x22

// CONSOLE_{BG}_{TXT}
#define CONSOLE_BLUE_RED       0x00 | 0x0C
#define CONSOLE_BLUE_GREEN     0x00 | 0x0A
#define CONSOLE_BLUE_WHITE     0x00 | 0x0F
#define CONSOLE_BLUE_MAGNETA   0x00 | 0x0D

static int receive_response_line(char* response_text, size_t capacity, HANDLE serial_handle) {
    size_t n = 0;
    unsigned char c;
    ULONGLONG deadline_ms = GetTickCount64() + 15000;
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

static int expect_response(HANDLE oonsole_handle, HANDLE serial_handle, const char* expected) {
    char response_text[256];

    if (!receive_response_line(response_text, sizeof(response_text), serial_handle)) {
        SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_RED);
        puts("Response timeout/error.");
        SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_WHITE);
        return 0;
    }
    if (strcmp(response_text, expected)) {
        SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_RED);
        printf("Expected [%s], received [%s]\n", expected, response_text);
        SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_WHITE);
        return 0;
    }
    return 1;
}

static int expect_response_format(HANDLE oonsole_handle, HANDLE serial_handle, const char* format, ...) {
    char expected[256];

    va_list args;
    va_start(args, format);

    int len = vsnprintf(expected, sizeof(expected), format, args);

    va_end(args);

    if (len < 0 || (size_t)len >= sizeof(expected)) {
        SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_RED);
        puts("Expected response format error.");
        SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_WHITE);
        return 0;
    }

    return expect_response(oonsole_handle, serial_handle, expected);
}

static int write_serial(HANDLE console_handle, HANDLE serial_handle, char* data, DWORD* dwBytesWrite) {
    int ret = 0;
    ret = WriteFile(serial_handle, data, strlen(data), &dwBytesWrite, NULL);
    if (ret) {
        /*SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_GREEN);
        printf("Data \"%.*s\" Successfully sent.\n", (int)(strcspn(data, "\n")), data);
        SetConsoleTextAttribute(oonsole_handle, CONSOLE_BLUE_WHITE);*/
    }
    else {
        SetConsoleTextAttribute(console_handle, CONSOLE_BLUE_GREEN);
        printf("Serial Write Failed Code :: %d", ret);
        SetConsoleTextAttribute(console_handle, CONSOLE_BLUE_WHITE);
    }
    return ret;
}

static int CRLF_to_LF(char* str) {
    char* src = str;
    char* dst = str;

    while (*src) {
        if (*src == '\r' && *(src + 1) == '\n') {
            *dst++ = '\n';
            src += 2;
        }
        else {
            *dst++ = *src++;
        }
    }
    *dst = '\0';

    return (int)(dst - str);
}

static void pt_progress_bar(HANDLE console_handle, size_t sent, size_t total) {
    const bar_size = 30;

    float percent = (total > 0) ? (float)sent / total : 1.0;
	int progress_bar = (int)(percent * bar_size);

    SetConsoleTextAttribute(console_handle, CONSOLE_BLUE_WHITE);
    
    printf("\r[");
    for (int i = 0; i < bar_size; i++) {
        if (i <= progress_bar) printf("*");
        else printf(" ");
    }
    printf("] %d%% (%d/%d bytes)", (int)percent * 100, sent, total);
	fflush(stdout);

    SetConsoleTextAttribute(console_handle, CONSOLE_BLUE_WHITE);

    if (sent >= total) printf("\n");
}

void main() {
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

    printf("***** Hydro Sensor File Edit Program *****\n");

    char inp[16] = {0,};
    printf("Enter Port Name:");
    scanf_s("%s", inp, sizeof(inp));

    int ch;
    while ((ch = getchar()) != '\n' && ch != EOF) {
    }

    char *portName = inp;
    printf("Port name is: %s\n", portName);

    HANDLE hSerial;
    
    hSerial = CreateFile(portName, 
                         GENERIC_READ | GENERIC_WRITE, 
                         0, 
                         0, 
                         OPEN_EXISTING, 
                         FILE_ATTRIBUTE_NORMAL, 
                         0);

    if (hSerial == INVALID_HANDLE_VALUE) {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_RED);
        if (GetLastError() == ERROR_FILE_NOT_FOUND)
        {
            printf("Serial port not found!\n");
        }
        else
        {
            printf("Serial port already in use!\n");
        }
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
        return;
    }
    else {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
        printf("%s Successfully opened !!\n", portName);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    DCB dcbSerialParams = { 0 };
    dcbSerialParams.DCBlength = sizeof(dcbSerialParams);

    if (!GetCommState(hSerial, &dcbSerialParams)) {
        // Error getting COM port state
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_RED);
        printf("Error : Getting COM port state\n");
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
        return;
    }
    else {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
        printf("Status successfully loaded from %s !!\n", portName);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    dcbSerialParams.BaudRate = CBR_115200;
    dcbSerialParams.ByteSize = 8;
    dcbSerialParams.StopBits = ONESTOPBIT;
    dcbSerialParams.Parity   = NOPARITY;

    if (!SetCommState(hSerial, &dcbSerialParams)) {
        // Error setting COM port state
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_RED);
        printf("Error : Setting COM port state\n");
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
        return;
    }
    else {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
        printf("Config successfully set to %s !!\n", portName);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    COMMTIMEOUTS timeouts = { 0 };
    timeouts.ReadIntervalTimeout = MAXDWORD;
    timeouts.ReadTotalTimeoutConstant = 500;
    timeouts.ReadTotalTimeoutMultiplier = 0;

    timeouts.WriteTotalTimeoutConstant = 50;
    timeouts.WriteTotalTimeoutMultiplier = 10;

    if (!SetCommTimeouts(hSerial, &timeouts)) {
        // Error set timeouts
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_RED);
        printf("Error : Setting timeouts\n");
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
        return;
    }
    else {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
        printf("Timeout successfulley set to %s !!\n", portName);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    /* ************************************************************************************************ */

    DWORD dwBytesWrite = 0;
    byte cmd[10] = {HEADER1, HEADER2, HEADER3, HEADER4, 0xCA, 0xFE, FOOTER1, FOOTER2, FOOTER3, FOOTER4};

    PurgeComm(hSerial, PURGE_RXCLEAR | PURGE_TXCLEAR);

    if (WriteFile(hSerial, cmd, 10, &dwBytesWrite, NULL)) {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
        printf("Start command successfully sent. \n", portName);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    Sleep(10);

    DWORD dwBytesRead = 0;
    int readSize = 4096;
    unsigned char buff[4096];

    const char marker[] = "EDITOR READY\n";
    size_t matched_length = 0;
    ULONGLONG deadline_ms = GetTickCount64() + 15000;
    while (matched_length < sizeof(marker) - 1 && GetTickCount64() < deadline_ms) {
        unsigned char c;
        DWORD n = 0;
        if (!ReadFile(hSerial, &c, 1, &n, NULL)) {
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

    if (ReadFile(hSerial, buff, readSize, &dwBytesRead, NULL)) {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_MAGNETA);
        for (DWORD i = 0; i < dwBytesRead; i++)
        printf("%c", buff[i]);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    int state = 0;

    while(1) {
        char fs_cmd[32] = {0,};
        char data[256] = {0, };
        
        if (state <= 1) printf("\nEnter Your Command: ");

        if (state == 7) fgets(data, sizeof(data), stdin);
        else fgets(fs_cmd, sizeof(fs_cmd), stdin);

        if (strcmp("ls\n", fs_cmd) == 0) state = 1;
        else if (strcmp("add\n", fs_cmd) == 0) state = 2;
        else if (strcmp("mod\n", fs_cmd) == 0) state = 3;
        else if (strcmp("del\n", fs_cmd) == 0) state = 4;
        else if (strcmp("ext\n", fs_cmd) == 0) state = 5;
        else if (strcmp("seek\n", fs_cmd) == 0) state = 8;
        else if (state != 6 && state != 7) state = 0;

        if (state == 7) {
            char len_buf[32];

            if (strncmp(data, "p\"", 2) == 0) {
                SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);

                int len = strlen(data);

                if (len >= 3) {
                    data[len - 2] = '\0';
                    char* result = data + 2;

                    memmove(data, result, strlen(result) + 1);
                }

                printf("Path %.*s detected.\n", (int)(strcspn(data, "\n")), data);

                FILE* fp;
                fp = fopen(data, "rb");

                if (fp == NULL) {
                    SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_RED);
                    printf("Cannot open file %.*s \n", (int)(strcspn(data, "\n")), data);
                }
                else {
                    fseek(fp, 0, SEEK_END);
                    long fileSize = ftell(fp);
                    rewind(fp);

                    sprintf_s(len_buf, sizeof(len_buf), "len: %d\n", fileSize);
                    write_serial(hConsole, hSerial, len_buf, &dwBytesWrite);

                    if (expect_response(hConsole, hSerial, "LEN_OK")) {
                        printf("fileSize: %d\n", fileSize);
                        char buffer[11];
                        int packetSent = 0;
                        int dataLeft = fileSize - packetSent;
                        int packetSize = (dataLeft >= 10) ? 10 : dataLeft;

                        if (fileSize > 10) {
                            size_t bytesRead;
                            while ((bytesRead = fread(buffer, 1, packetSize, fp)) > 0) {
                                write_serial(hConsole, hSerial, buffer, &dwBytesWrite);

                                Sleep(10);

                                if (expect_response_format(hConsole, hSerial, "ACK_%db", packetSize))
                                {
									packetSent += packetSize;
                                    dataLeft -= packetSize;

                                    packetSize = (dataLeft >= 10) ? 10 : dataLeft;
                                    buffer[packetSize] = '\0';

									pt_progress_bar(hConsole, packetSent, fileSize);
                                }
                                else {
                                    printf("Error occured while transfer");
                                    break;
                                }
                            }
                        }
                        else {
                            fgets(buffer, sizeof(buffer), fp);
                        }
                    }

                    fclose(fp);
                }

                SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
			}
            else {
                data[strlen(data) - 1] = '\0';

                sprintf_s(len_buf, sizeof(len_buf), "len: %d\n", strlen(data));
                write_serial(hConsole, hSerial, len_buf, &dwBytesWrite);

                if (expect_response(hConsole, hSerial, "LEN_OK")) {
                    if (strlen(data) > 10) {
                        char buffer[11];
                        int packetSent = 0;
						int dataLeft = strlen(data) - packetSent;

                        while (dataLeft) {
							int packetSize = (dataLeft >= 10) ? 10 : dataLeft;
                            buffer[packetSize] = '\0';
							memcpy(buffer, data + packetSent, packetSize);
                            write_serial(hConsole, hSerial, buffer, &dwBytesWrite);

                            Sleep(10);
                            if (expect_response_format(hConsole, hSerial, "ACK_%db", packetSize))
                            {
                                packetSent += packetSize;
                                dataLeft -= packetSize;

								pt_progress_bar(hConsole, packetSent, strlen(data));
                            }
                            else {
                                printf("Error occured while transfer");
                                packetSent = dataLeft + 1;
                            }
                        }
                    }
                    else {
                        write_serial(hConsole, hSerial, data, &dwBytesWrite);
                        if (expect_response_format(hConsole, hSerial, "ACK_%db", strlen(data)))
                        {
                            SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
                            printf("%d bit Successfully sent.\n", strlen(data));
                            SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
                        }
                    }
                }
            }

            state = 0;
        }
        else {
            if (WriteFile(hSerial, fs_cmd, strlen(fs_cmd), &dwBytesWrite, NULL)) {
                SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
                printf("Command \"%.*s\" Successfully sent.\n", (int)(strcspn(fs_cmd, "\n")), fs_cmd);
                SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
            }
        }
        
        if (state == 5) {
            SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_RED);
            printf("Exit Calibration Edit.");
            SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
            return;
        }

        Sleep(100);

        if (ReadFile(hSerial, buff, readSize, &dwBytesRead, NULL)) {
            SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_MAGNETA);
            for (DWORD i = 0; i < dwBytesRead; i++)
            printf("%c", buff[i]);
            SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
        }

        if (state == 3) {
            state = 6;
            continue;
        }

        if (state == 6) state++;
    }
error:
    puts("Communication/session failed. Close this program, reset board, then reconnect.");
    CloseHandle(hSerial);
    return 1;
}