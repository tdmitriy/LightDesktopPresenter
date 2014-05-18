package com.ldp.androidclient.utils.user_preferences;

public class LdpConnectionPreferences {

    private String ipAddress;
    private String displayedName;

    public String getIPAddress() {
        return ipAddress;
    }

    public void setIPAddress(String ipAddress) {
        this.ipAddress = ipAddress;
    }

    public String getDisplayedName() {
       return displayedName;
    }

    public void setDisplayedName(String displayedName) {
        this.displayedName = displayedName;
    }
}
