@echo Starting copy from %1 to %2
@echo Copying runtime libs...

@xcopy %1SDL2-2.0.8\lib\x64\*.dll %2 /C /D /Y /K /I
@xcopy %1SDL2_ttf-2.0.14\lib\x64\*.dll %2 /C /D /Y /K /I
@xcopy %1SDL2_image-2.0.3\lib\x64\*.dll %2 /C /D /Y /K /I
@xcopy %1SDL2_mixer-2.0.4\lib\x64\*.dll %2 /C /D /Y /K /I
@xcopy %1SDL2_image-2.0.3\lib\x64\zlib1.dll %2 /C /Y /K /I

@echo Copying assets...
@xcopy %1assets\*.ttf %2assets /C /D /Y /K /I
@xcopy %1assets\*.png %2assets /C /D /Y /K /I
@xcopy %1assets\*.wav %2assets /C /D /Y /K /I