1. Open the solution in Visual Studio
2. Re-build the entire solution
3. Publish the "Nop.Web" project from Visual Studio
4. For Linux, set execute permission for wkhtmlpdf file for pdf generation
```shell
chmod +x Rotativa/Linux/wkhtmltopdf
```
5. Ignore paths in deployment
  - wwwroot/_content

P.S. When publishing ensure that configuration is set to "Release"