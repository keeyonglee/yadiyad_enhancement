rem load js, css, img for YadiYad.Pro.Web
mkdir "wwwroot\js"
mkdir "wwwroot\css"
mkdir "wwwroot\img"

rmdir -rf "wwwroot\js\pro"
rmdir -rf "wwwroot\css\pro"
rmdir -rf "wwwroot\img\pro"

mklink /D "wwwroot\js\pro" "..\..\..\..\YadiYad.Pro\YadiYad.Pro.Web\wwwroot\js\pro\"
mklink /D "wwwroot\css\pro" "..\..\..\..\YadiYad.Pro\YadiYad.Pro.Web\wwwroot\css\pro\"
mklink /D "wwwroot\img\pro" "..\..\..\..\YadiYad.Pro\YadiYad.Pro.Web\wwwroot\img\pro\"

rem push pro areas views to YadiYad.Pro.Web
mkdir "..\..\YadiYad.Pro\YadiYad.Pro.Web\Areas\Pro"
mklink /D ".\..\..\YadiYad.Pro\YadiYad.Pro.Web\Areas\Pro\Views" "..\..\..\..\Presentation\Nop.Web\Areas\Pro\Views" 
mklink /D ".\..\..\YadiYad.Pro\YadiYad.Pro.Web\wwwroot\lib" "..\..\..\Presentation\Nop.Web\wwwroot\lib" 

rem copy js and css library from YadiYad.Pro.Web to Nop.Web
rem xcopy /B /S /E /Y "..\..\YadiYad.Pro\YadiYad.Pro.Web\wwwroot\lib\*" "wwwroot\lib\"

pause