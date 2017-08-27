# Protobuild.Manager

A cross-platform GUI manager for Protobuild projects.  It's goals are:

  * Make it easy to create cross-platform projects from templates
  * Allow developers using Protobuild to easily change the platform they're working on

![Opening screen](http://i.imgur.com/IlsCmEf.png)

## Installation

### Windows

On Windows, we provide a web installer.  You need an internet connection while the installer is running to complete the installation.

[Download for Windows](https://github.com/Protobuild/Protobuild.Manager/releases/download/latest/ProtobuildWindowsInstall.exe)

### macOS

On macOS, we provide a script to complete the installation.  You will need to download and install Mono first:

[Download Mono for macOS](http://www.mono-project.com/download/#download-mac)

After you have installed Mono, open a terminal, and then run:

```bash
curl -L https://github.com/Protobuild/Protobuild.Manager/releases/download/latest/ProtobuildMacOSInstall.sh | bash
```

### Linux

On Linux, we provide a script to complete the installation.  Depending on your distro, you may need to install dependency packages.  Instructions for known distributions are listed below:

#### Arch Linux
```bash
# Install Mono and GTK# 3 with the following command:
sudo pacman -S mono gtk-sharp-3 webkitgtk3
# Run the script to install Protobuild Manager:
curl -L https://github.com/Protobuild/Protobuild.Manager/releases/download/latest/ProtobuildLinuxInstall.sh | bash
```

#### Fedora

```bash
# Install Mono and GTK# 3 with the following command:
sudo dnf install mono-core gtk-sharp3
# Run the script to install Protobuild Manager:
curl -L https://github.com/Protobuild/Protobuild.Manager/releases/download/latest/ProtobuildLinuxInstall.sh | bash
```

#### OpenSUSE

```bash
# Install Mono, GTK# 3 and WebKit GTK 3 with the following command:
sudo zypper in mono-core gtk-sharp3 libwebkit2gtk-4
# Run the script to install Protobuild Manager:
curl -L https://github.com/Protobuild/Protobuild.Manager/releases/download/latest/ProtobuildLinuxInstall.sh | bash
```

#### Ubuntu

```bash
# Make sure you have the universe repository enabled first:
sudo add-apt-repository "deb http://archive.ubuntu.com/ubuntu $(lsb_release -sc) universe"
sudo apt update
# Install Mono, GTK# 3, WebKit GTK 3 and extra libraries with the following command:
sudo apt install mono-runtime gtk-sharp3 libwebkit2gtk-4.0-37 libmono-system-web-extensions4.0-cil
# Run the script to install Protobuild Manager:
curl -L https://github.com/Protobuild/Protobuild.Manager/releases/download/latest/ProtobuildLinuxInstall.sh | bash
```

#### Other Distros

If you know of the commands required to install dependencies on other Linux distributions, please submit a pull request to update this documentation.

## How to Contribute

Please refer to the [Protobuild](https://github.com/Protobuild/Protobuild) repository for instructions on how to contribute.

## License Information

Protobuild Manager is licensed under the MIT license.

```
Copyright (c) 2015 Various Authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
```

## Community Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).
