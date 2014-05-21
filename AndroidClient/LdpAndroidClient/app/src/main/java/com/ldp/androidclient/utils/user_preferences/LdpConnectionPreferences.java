package com.ldp.androidclient.utils.user_preferences;

public class LdpConnectionPreferences {

    private String ipAddress;
    private String connectionName;
    private String password;

    public String getIPAddress() {
        return ipAddress;
    }

    public void setIPAddress(String ipAddress) {
        this.ipAddress = ipAddress;
    }

    public String getPassword() {
        return password;
    }

    public String getConnectionName() {
       return connectionName;
    }

    public void setConnectionName(String connectionName) {
        this.connectionName = connectionName;
    }

    public void setPassword(String password) {
        this.password = password;
    }
}
