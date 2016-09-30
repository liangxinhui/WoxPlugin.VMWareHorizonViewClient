# WoxPlugin.VMWareHorizonViewClient

【应用场景】

多显示器用户，运行VMWare Horizon View Client时，可以指定（单屏）全屏，或者所有显示器。

实际使用时，两种模式之间的切换比较麻烦。需要进入多个菜单进行设置后重新登录。

【解决方案】

借助Wox，启动前修改配置文件，调整显示模式参数

【实现细节】

VMware Horizon View Client 配置文件

‪C:\Users\******\AppData\Roaming\VMware\VMware Horizon View Client\prefs.txt

```xml
<?xml version="1.0"?>
<Root>
  <RecentServer serverName="*.*.*.*" serverType="view" mruUsername="******" mruDomain="******" lastLogInAsCurrentUser="false" localIMEEnable="false" serverGuid="******" serverCacheDir="C:\Users\******\AppData\Roaming\VMware\VMware Horizon View Client\App Cache\******\">
    <RecentDesktop desktopID="cn=win81,ou=applications,dc=vdi,dc=vmware,dc=int" connectUSBOnStartup="false" connectUSBOnInsert="false" protocol="RDP">
      <LastDisplaySize displaySize="FullScreen"/>
      <LastMonitor monitorIndex="0"/>
    </RecentDesktop>
    <FileRedirection/>
  </RecentServer>
  <SecurityMode certCheckMode="2"/>
  <LastLoginAsCurrentUser loginAsCurrentUser="true"/>
  <GlobalDefaultServer serverName="*.*.*.*"/>
  <BrokerJumpList>
    <BrokerJump BrokerName="*.*.*.*" BrokerArguments="vmware-view://*.*.*.*/?"/>
  </BrokerJumpList>
  <DesktopJumpList>
    <DesktopJump DesktopName="******" DesktopArguments="vmware-view://******@******/cn=******,ou=applications,dc=vdi,dc=vmware,dc=int?&amp;domainName=******&amp;desktopProtocol=RDP&amp;desktopLayout=fullscreen"/>
  </DesktopJumpList>
  <AutoConnectServer AutoConnectServerName="******"/>
  <sharingList showPromptDlg="true" shareHomeDirectory="false" allowAccessRemovable="false"/>
  <BlastSettings/>
</Root>
```

desktopLayout可选的配置项：
  - multimonitor：所有显示器
  - fullscreen：全屏

Root/ RecentServer /RecentDesktop  节点下有个LastDisplaySize ：
  - 全屏需要LastDisplaySize节点
  - 非全屏不能有LastDisplaySize节点
