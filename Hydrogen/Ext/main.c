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

    if (WriteFile(hSerial, cmd, 10, &dwBytesWrite, NULL)) {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
        printf("Start command successfully sent. \n", portName);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    Sleep(10);

    DWORD dwBytesRead = 0;
    int readSize = 4096;
    unsigned char buff[4096];

    if (ReadFile(hSerial, buff, readSize, &dwBytesRead, NULL)) {
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_MAGNETA);
        for (DWORD i = 0; i < dwBytesRead; i++)
        printf("%c", buff[i]);
        SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
    }

    int state = 0;

    while(1) {
        char fs_cmd[32] = {0,};
        char data[4096] = {0, };
        
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
            if (WriteFile(hSerial, data, strlen(data), &dwBytesWrite, NULL)) {
                SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_GREEN);
                printf("Data \"%.*s\" Successfully sent.\n", (int)(strcspn(data, "\n")), data);
                SetConsoleTextAttribute(hConsole, CONSOLE_BLUE_WHITE);
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
}