# Utils

A collection of Unity Editor utilities, packaged for easy installation via Git URL.

## Easy Scene Switcher

Easy Scene Switcher adds a toolbar dropdown for quickly opening:

* **Build Scenes** (from your Editor Build Settings)
* **Other Scenes** (manually tracked scenes persisted between sessions)

### Features

* Split scenes into two groups with headers
* Scene list settings Scriptable
* One-click scene switching with save prompts

### Requirements

* Unity 2020.3 or newer
* [unity-toolbar-extender](https://github.com/marijnz/unity-toolbar-extender/blob/master)

### Installation

1. Open your Unity project and go to **Window > Package Manager**.

2. Click **+** and select **Add package from Git URL…**.

3. Paste the URL below and click **Add**:

   ```
   https://github.com/JustGerome/EasySceneSwitcher.git#v1.0.2
   ```

4. If **unity-toolbar-extender** is not installed, repeat the steps with:

   ```
   https://github.com/marijnz/unity-toolbar-extender.git#v1.4.2
   ```

### Usage

After installation, a dropdown appears in the top toolbar showing your current scene. Click the dropdown to expand and switch scenes:

```text
▶ CurrentScene
   Build Scenes
     • SceneA
     • SceneB
   Other Scenes
     • DevScene
     • TestScene
   Manage Other Scenes
```

To add a new scene under **Manager Other Scenes**, add scenes through the **Other Scene** List.

### Contributing

1. Fork this repository.
2. Create a branch:

   ```bash
   git checkout -b feature/YourFeature
   ```
3. Commit your changes:

   ```bash
   git commit -m "Add feature"
   ```
4. Push to your fork:

   ```bash
   git push origin feature/YourFeature
   ```
5. Open a Pull Request.

### License

This project is licensed under the MIT License. See [LICENSE](../LICENSE) for details.
