Light Desktop Presenter (Graduate coursework)
=====================

This project provides remote desktop and volume control for OS Windows via Android over TCP Protocol.

##Features

* Desktop sharing
* Mouse/Keyboard controlling
* Volume Controling
* Shutdown/Restart

##Technologies

Server side (C#):
- [Google Protocol Buffers](https://developers.google.com/protocol-buffers/?hl=en) (to establish communication between platforms)
- CoreAudioApi (windows volume controlling api)
- SharpDX (DirectX wrapper for capturing desktop screenshots)
- LZ4 (compress/decompress lib to reduce network traffic)

Client side (Android Java):
- Google Protocol Buffers
- LZ4


