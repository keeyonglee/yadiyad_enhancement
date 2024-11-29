mkdir wwwroot/js
mkdir wwwroot/css
mkdir wwwroot/img

rm wwwroot/js/pro
rm wwwroot/css/pro
rm wwwroot/img/pro

ln -s ../../../../YadiYad.Pro/YadiYad.Pro.Web/wwwroot/js/pro wwwroot/js/pro
ln -s ../../../../YadiYad.Pro/YadiYad.Pro.Web/wwwroot/css/pro wwwroot/css/pro
ln -s ../../../../YadiYad.Pro/YadiYad.Pro.Web/wwwroot/img/pro wwwroot/img/pro

rm ../../YadiYad.Pro/YadiYad.Pro.Web/Areas/Pro/Views
rm ../../YadiYad.Pro/YadiYad.Pro.Web/wwwroot/lib
ln -s ../../../../Presentation/Nop.Web/Areas/Pro/Views ../../YadiYad.Pro/YadiYad.Pro.Web/Areas/Pro/Views
ln -s ../../../Presentation/Nop.Web/wwwroot/lib ../../YadiYad.Pro/YadiYad.Pro.Web/wwwroot/lib
