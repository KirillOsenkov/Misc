@echo off
setlocal

set "directory=C:\treetool"

if exist "%directory%" (
    rmdir /s /q "%directory%"
)

md "%directory%"
cd "%directory%"
md a b c
cd a
md a1 a2
cd ..
cd b
md b1
cd b1
md b11 b12
cd ..\..
cd c
md c1
cd "%directory%"
tree
