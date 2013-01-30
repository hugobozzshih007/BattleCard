mkdir tmp
move Library\*.asset tmp\
rmdir /S /Q Library
mkdir Library
move tmp\*.* Library
rmdir /S /Q tmp
