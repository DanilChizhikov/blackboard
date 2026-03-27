# Blackboard
[![Unity Version](https://img.shields.io/badge/unity-2022.3+-000.svg)](https://unity3d.com/get-unity/download/archive)
![Unity Tests](https://github.com/DanilChizhikov/blackboard/actions/workflows/tests.yml/badge.svg?branch=master)

## Overview

## Table of Contents
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Manual Installation](#manual-installation)
    - [UPM Installation](#upm-installation)
- [Features](#features)
- [Settings](#settings)
- [Usage](#usage)
- [License](#license)

## Getting Started

### Prerequisites
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Manual Installation
1. Download the .unitypackage from the [releases](https://github.com/DanilChizhikov/blackboard/releases/) page.
2. Import com.dtech.blackboard.x.x.x.unitypackage into your project.

### UPM Installation
1. Open the manifest.json file in your project's Packages folder.
2. Add the following line to the dependencies section:
    ```json
    "com.dtech.blackboard": "https://github.com/DanilChizhikov/blackboard.git",
    ```
3. Unity will automatically import the package.

If you want to set a target version, Logging uses the `v*.*.*` release tag so you can specify a version like #v0.0.1.

For example `https://github.com/DanilChizhikov/blackboard.git#v0.0.1`.

## Features

## Settings

## Usage

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.