<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
    <xsl:output method="xml" indent="yes" />
    <xsl:strip-space elements="*"/>

    <xsl:template match='/'>
        <Wix xmlns="http://schemas.microsoft.com/wix/2006/wi" xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">

            <Product Id="*" Name="PushNotifications" Language="1033" Version="0.1.2" Manufacturer="Elders" UpgradeCode="81c22642-b0ed-41e1-a37d-b3c6c7975e4b">

                <Package Id='*'
                         Manufacturer='Elders'
                         Description="Installs the Push Notifications service"
                         Comments='(c) 2015 Elders'
                         InstallerVersion='500'
                         Languages='1033'
                         SummaryCodepage='1252'
                         Compressed='yes'
                         AdminImage='no'
                         Platform='x86'
                         ReadOnly='yes'
                         ShortNames='no'
                         InstallScope='perMachine' />

                <MediaTemplate EmbedCab="yes" />

                <Upgrade Id="81c22642-b0ed-41e1-a37d-b3c6c7975e4b">
                    <UpgradeVersion Minimum="0.1.0.0" IncludeMinimum="yes"
                                    Maximum="0.99.99.99" IncludeMaximum="yes"
                                    OnlyDetect="no"
                                    Language="1033"
                                    Property="PREVIOUSVERSIONSINSTALLED" />
                </Upgrade>

                <InstallExecuteSequence>
                    <RemoveExistingProducts Before='InstallInitialize' />
                </InstallExecuteSequence>

                <Feature Id="ServicesGroup" Title="Services" Level="1">
                    <Feature Id="PushNotificationsServiceFeature" Title="PushNotifications windows service" Level="1">
                        <ComponentGroupRef Id="PushNotificationsServiceComponents" />
                    </Feature>
                </Feature>

                <UI Id="WixUI_MondoNoLicense">
                    <TextStyle Id="WixUI_Font_Normal" FaceName="Tahoma" Size="8" />
                    <TextStyle Id="WixUI_Font_Bigger" FaceName="Tahoma" Size="12" />
                    <TextStyle Id="WixUI_Font_Title" FaceName="Tahoma" Size="9" Bold="yes" />

                    <Property Id="DefaultUIFont" Value="WixUI_Font_Normal" />
                    <Property Id="WixUI_Mode" Value="Mondo" />

                    <DialogRef Id="ErrorDlg" />
                    <DialogRef Id="FatalError" />
                    <DialogRef Id="FilesInUse" />
                    <DialogRef Id="MsiRMFilesInUse" />
                    <DialogRef Id="PrepareDlg" />
                    <DialogRef Id="ProgressDlg" />
                    <DialogRef Id="ResumeDlg" />
                    <DialogRef Id="UserExit" />

                    <Publish Dialog="ExitDialog" Control="Finish" Event="EndDialog" Value="Return" Order="999"></Publish>

                    <Publish Dialog="WelcomeDlg" Control="Next" Event="NewDialog" Value="CustomizeDlg"></Publish>
                    <Publish Dialog="CustomizeDlg" Control="Back" Event="NewDialog" Value="WelcomeDlg"></Publish>
                    <Publish Dialog="CustomizeDlg" Control="Next" Event="NewDialog" Value="VerifyReadyDlg"></Publish>

                    <Publish Dialog="VerifyReadyDlg" Control="Back" Event="NewDialog" Value="CustomizeDlg"></Publish>
                </UI>

                <UIRef Id="WixUI_Common" />

            </Product>

            <Fragment>
                <Directory Id="TARGETDIR" Name="SourceDir">
                    <Directory Id="ProgramFilesFolder">
                        <Directory Id="CompanyFolder" Name="Elders">
                            <Directory Id="ProductFolder" Name="PushNotifications">
                                <Directory Id="PushNotificationsINSTALLFOLDER" Name="PushNotificationsService" />
                            </Directory>
                        </Directory>
                    </Directory>
                    <Directory Id="TempFolder"/>
                </Directory>

            </Fragment>

            <Fragment>
                <ComponentGroup Id="PushNotificationsServiceComponents">
                    <Component Id="PushNotificationsServiceComponent" Directory="PushNotificationsINSTALLFOLDER" Guid="60e92385-6902-4694-a59c-fde0c94e67dd">
                        <xsl:call-template name="PushNotificationsReferencesTemplate" />
                        <ServiceInstall
                            Id="PushNotificationsServiceInstaller"
                            Type="ownProcess"
                            Vital="yes"
                            Name="PushNotifications"
                            DisplayName="PushNotifications"
                            Description="Push Notifications"
                            Start="auto"
                            Account="NT AUTHORITY\LocalService"
                            ErrorControl="normal"
                            Interactive="no"
                        >
                            <!--<ServiceDependency Id="RabbitMQ"/>-->
                            <util:PermissionEx
                                    User="Authenticated Users"
                                    GenericAll="yes"
                                    ServiceChangeConfig="yes"
                                    ServiceEnumerateDependents="yes"
                                    ChangePermission="yes"
                                    ServiceInterrogate="yes"
                                    ServicePauseContinue="yes"
                                    ServiceQueryConfig="yes"
                                    ServiceQueryStatus="yes"
                                    ServiceStart="yes"
                                    ServiceStop="yes" />
                        </ServiceInstall>
                        <ServiceControl Id="ServiceControl_Start" Name="PushNotifications" Start="install" Stop="both" Remove="both" Wait="yes" />
                    </Component>
                </ComponentGroup>
            </Fragment>
        </Wix>
    </xsl:template>

    <xsl:template name="PushNotificationsReferencesTemplate" match="@*|node()">
        <xsl:copy>
            <xsl:for-each select="wix:Wix/wix:Fragment/wix:ComponentGroup/wix:Component/wix:File[@Source and not (contains(@Source, '.pdb')) and not (contains(@Source, '\CodeContracts\')) and not (contains(@Source, '.vshost.')) and (contains(@Source,'PushNotifications.WS'))]">
                <xsl:apply-templates select="."/>
            </xsl:for-each>
        </xsl:copy>
    </xsl:template>

    <xsl:template match="wix:Wix/wix:Fragment/wix:ComponentGroup/wix:Component/wix:File">
        <xsl:copy>
            <xsl:choose>
                <xsl:when test="not (contains(@Source, 'PushNotifications.WS.exe')) or (contains(@Source, '.config'))">
                    <xsl:apply-templates select="@*[name()!='KeyPath']"/>
                    <xsl:attribute name="Vital">
                        <xsl:value-of select="'yes'"/>
                    </xsl:attribute>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:apply-templates select="@*"/>
                    <xsl:attribute name="Vital">
                        <xsl:value-of select="'yes'"/>
                    </xsl:attribute>
                </xsl:otherwise>
            </xsl:choose>
        </xsl:copy>
    </xsl:template>

</xsl:stylesheet>
