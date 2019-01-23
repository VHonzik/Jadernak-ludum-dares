@echo off
for /f %%i in ('git rev-parse --short HEAD') do set sha=%%i
(
@echo #pragma once
@echo namespace LD43
@echo {
@echo const char kBuildHash[] = "%sha%";
@echo }
) > %1