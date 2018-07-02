**MinApp** is a library for .NET Framework (targeted mainly at WPF Windows Application) to make UI layer for apps using Web-stack technologies: HTML5, CSS3 and Javascript (instead of Microsoft XAML).

You can make an app UI in Web languages while have complete access to .NET Framework Windows API for the backend. If you are familiar with ASP.NET MVC, you will feel yourself right at home.

# Why MinApp

* You prefer HTML5 and CSS3 over XAML for layout, formatting, animations etc...
* With MinApp, you can test your product on your favorite web browser (Chrome) and deploy it as WPF app later.
* Unlike Web platforms like Ionic, you don't have to pack entire Web engine (which can be up to 70MB). MinApp uses the Windows built-in web browser (IE 10 and 11 depends on the OS).
* You have complete access to .NET Framework API and do not have to learn new APIs.

# Compatibility

If you plan to support Windows 7, your HTML5, CSS3 and Javascript should support IE10. This can be done using Babel and polyfills to ease the development. For Windows 8.1 and 10, the latest IE11 is available.

# Installation

[MinApp Nuget Package](https://www.nuget.org/packages/MinApp/):

`Install-Package MinApp`

# Usage and Getting Started

Please check [the wiki](https://github.com/datvm/MinApp/wiki) for usage and documentation.