# Thunderbolt Switch

![image](https://user-images.githubusercontent.com/934757/122594143-eb6d9100-d066-11eb-9215-748ce08b005e.png)

Thunderbolt Switch, a software that switches your game settings and device power profiles (TDP, Power Balance, Voltage) for all your games based on device current status (AC, DC, EGPU, External Screen). Now compatible with both Intel and AMD processors.

## Project Status

Refer to https://github.com/Valkirie/Thunderbolt-Switch/wiki/Status for the updated project status.

## Guide

**Database**

The database stores every information related to your games or applications managed by Thunderbolt Switch. You can either add new applications to your database by Manually Selecting them or use the (Beta) Automatic Detection.

- Click on "Database".
- Click on "Select Manually" and pick the the game executable.
- Click on "Automatic Detection" and wait until Thunderbolt Switch asks you which software you'd like to add to the list.

**Settings**

Settings menu has not yet been implemented. All your Thunderbolt Switch settings are stored inside "DockerForm.exe.config".
From there you can change a few settings:
- MinimizeOnStartup : If True, software will start minimized.
- MinimizeOnClosing : If True, software will get minimized instead of closing when using the top right cross.
- BootOnStartup : If True, software will be automatically starts on Logon.
- ImageHeight / ImageWidth : Defines the size of executable image inside the application list.
- MonitorProcesses : If True, software will detect database application start/stop and manage bounds power profile and settings.
- IGDBListLength : Defines the number of returned application details suggestions when using "Download from IGDB" from the Game Properties window.
- ToastNotifications : If True, windows will display notifications on specific software events.
- SaveOnExit : If True, software will force save all your current settings on software exit.
- MonitorThreadRefresh : How often should the main thread be called (in milliseconds).
- MonitorProfiles : If True, an extra thread will be used to monitor modifications made to the Power Profiles xml files.

**Properties**

![Visual](assets/properties.png)

The Game Properties window will be displayed and will try to collect as much information as possible from your executable, including: Game Name, Game Developer, Game Version and Game Visual. To link your application to IGDB and automatically gather details from there database, right click on the Game Visual and select "Download from IGDB". The IGDB search will be based on the "Name" settings field. Right clicking on the Settings tab will allow you to change game specific Parameters, link Files and Registries to your application or attribute specific Power Profiles to it.

- To attribue a Settings File or Registry to your application, right click on the Settings tab and click on "Create Setting" then select the setting type from the list: File or Registry depending on where your application settings are stored. (*If you need help locating your application settings location, right click on the Application Visual and select "Search on PCGaming Wiki"*).
- To change game specific Parameters, click on the General tab.
- To attribute game specific Power Profiles, click on the Power Profiles tab and check those that you wish to turn on/off automatically on application start/exit.

**Application List**

The Game List will display all your currently handled applications. From that list, you can right click on any title to display the Action Menu.
From where you can quickly:
- Start the Game
- Open the Game Location
- Open each Settings Location
- Remove the Game from the Database
- Open the Game Properties window

**Power Profile**

![Visual](assets/trayicon.png)

Power Profiles contains configurable Power Information and are stored are xml files inside the /profiles/ folder. Power Profiles can be enabled through User-defined Triggers (ApplyMask), be bounds to a specific Game to finely adapt your device power, CPU and GPU capacities based on your usage scenario or forced applied by right clicking on the taskbar icon. (*Use the default.xml template to create as many Power Profiles as you want, do not delete it*).

Power Information:
- Turbo Boost Long Power Max
- Turbo Boost Short Power Max
- Adaptive Voltage (CPU Core, CPU Cache, System Agent, Intel GPU)
- Intel Power Balance

User-defined Triggers (*ApplyMask value. Can be added together. ApplyMask 3 would trigger the Power Profile on both Battery/Plugged In Power Status*).
- When device power status has changed (*32: On Status Change, 1: On Battery, 2: Plugged In*)
- When device docking status has changed (*4: External GPU Plugged In*)
- When device starts (*8: On Startup*)
- When device video output status has changed (*22: HDMI Plugged In*)
