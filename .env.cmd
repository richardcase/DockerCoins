:: Currentt directory
SET DCKCOINS=%cd%

:: Set the code and 3rd party library path
SET DCCODE=%DCKCOINS%\code
SET DCTHIRDPARTY=%DCKCOINS%\third_party

:: Set the GOPATH and PATH
SET GOPATH=%DCTHIRDPARTY%;%DCCODE%
SET PATH=%DCCODE%\bin;%DCTHIRDPARTY%\bin;%PATH%
