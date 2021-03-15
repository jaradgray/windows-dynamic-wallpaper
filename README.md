# Windows Dynamic Wallpaper

Windows Dynamic Wallpaper is a personalization app that automatically changes your desktop background throughout the day based on your location.

![Dynamic Wallpaper Demo](/demo.gif)

## Building

To build the project locally:

1. Clone or download the project's source to your machine
2. Open the solution in Visual Studio
3. Build the solution

Note: this project was written using .NET Framework 4.6.1

## Usage

After launching the app, you'll need to enter your location and select a wallpaper. See the Creating a Dynamic Wallpaper section below for info about how to create dynamic wallpapers the app can use.

Note that the app runs in the background, so closing its window doesn't stop its process. To reopen the app or stop its process, interact with its system tray icon.

## Creating a Dynamic Wallpaper

Unlike the Dynamic Desktop feature in macOS, which bundles a dynamic wallpaper's images and metadata in a single `.heic` file, Windows Dynamic Wallpaper requires a dynamic wallpaper's images and metadata to be stored as separate files in a directory. Although this implementation isn't as elegant, it makes it much easier to create your own dynamic wallpapers.

To create a dynamic wallpaper, create a directory containing (1) one or more images, and (2) a properly formatted file named `manifest.json`.

### The `manifest.json` file

`manifest.json` is a JSON file that defines the dynamic wallpaper contained in its parent directory. It consists of a JSON object with keys for the dynamic wallpaper's name and an array of objects defining the dynamic wallpaper's individual images.

An example `manifest.json` file might look like the following:

```json
{
    "name": "My Dynamic Wallpaper",
    "images": [
        {
            "name": "sunrise.jpg",
            "progress": 0
        },
        {
            "name": "midday.jpg",
            "progress": 90
        },
        {
            "name": "sunset_start.jpg",
            "progress": 179.31
        },
        {
            "name": "dusk.jpg",
            "progress": 187
        }
    ]
}
```

Take note of the following in the above code:
- `manifest.json` defines a JSON object with two keys, `"name"` and `"images"`
    - `"name"` is a string whose value is whatever you decide to name your dynamic wallpaper
    - `"images"` is an array of JSON objects
- Each object in the `"images"` array contains two keys, `"name"` and `"progress"`
    - `"name"` is a string whose value is the name of an image file in the same directory as `manifest.json`
    - `"progress"` is a floating-point number (this value associates the image with the sun's position and is explained below)

You can think of the outer JSON object as representing a dynamic wallpaper as a whole, and each object in the `"images"` array as representing an individual image that is to be shown at a specific time (defined by its `progress` value).

### An image's `progress` value

Unlike macOS's dynamic desktop feature, which associates a picture with the sun's altitude and azimuth at the time the picture was taken, Windows Dynamic Desktop conceptualizes the sun's position as a value called `progress`.

`progress` is a floating-point number between 0 (inclusive) and 360 (exclusive) that represents the sun's progress through the day, where 0 corresponds to sunrise, 90 to solar noon, 180 to sunset, and 270 to nadir (i.e. when the sun is at its lowest point below the horizon). The app uses this `progress` value to calculate the time the sun will reach that progress today, and then changes the desktop background to that image at the calculated time.

Note: to see the association between the various sun phases (e.g. sunrise, golden hour, dusk, etc.), their corresponding times and `progress` values for the current day, and thier average `progress` values over the next year, click the Sun Info icon at the far right of the status bar.

![Indicate Sun Info button](/indicate_sun_info.jpg)

## Credits

Windows Dynamic Wallpaper could not have been made without ideas and solutions provided by these smart and generous people:

- [SunCalc](https://github.com/mourner/suncalc) - an amazing library that does all of the complicated calculations regarding sun position (plus other cool stuff. Check it out!). This library makes this app possible
- [SunCalc-Net](https://github.com/kostebudinoski/SunCalcNet) - a .NET port of the SunCalc library
- These three articles by [Marcin Czachurski](https://medium.com/@mczachurski) exploring the `.heic` image file format, the structure of the dynamic wallpaper files that ship with macOS Mojave, and how to create dynamic wallpapers for macOS:
    - [macOS Mojave dynamic wallpaper](https://itnext.io/macos-mojave-dynamic-wallpaper-fd26b0698223)
    - [macOS Mojave dynamic wallpaper (II)](https://itnext.io/macos-mojave-dynamic-wallpapers-ii-f8b1e55c82f)
    - [macOS Mojave dynamic wallpaper (III)](https://itnext.io/macos-mojave-wallpaper-iii-c747c30935c4)
- [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) for handling JSON data in .NET
