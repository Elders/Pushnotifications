<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
    <xsl:output method="xml" indent="yes" />
    <xsl:strip-space elements="*"/>

    <xsl:variable name="CompanyName">Elders</xsl:variable>
    <xsl:variable name="ProductName">Elders.PushNotifications</xsl:variable>
    <xsl:variable name="UpgradeCode">4d41074f-ae42-4b45-be25-4759a2c43a0e</xsl:variable>

    <xsl:variable name="Component1">PushNotifications.WS</xsl:variable>
    <xsl:variable name="Component1_Title">Elders.PushNotifications</xsl:variable>
    <xsl:variable name="Component1_ServiceExe">PushNotifications.WS.exe</xsl:variable>
    <xsl:variable name="Component1_ServiceName">Elders.PushNotifications</xsl:variable>
    <xsl:variable name="Component1_ServiceDescr">Elders PushNotifications</xsl:variable>

    <xsl:template match='/'>
        <Wix xmlns="http://schemas.microsoft.com/wix/2006/wi" xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">

            <Product Id="*" Name="{$ProductName}" Language="1033" Version="1.1.1.0" Manufacturer="{$CompanyName}" UpgradeCode="{$UpgradeCode}">

                <Package Id='*'
                         Description="PackageDescription"
                         Comments='Comments'
                         Manufacturer='{$CompanyName}'
                         InstallerVersion='200'
                         Languages='1033'
                         SummaryCodepage='1252'
                         Compressed='yes'
                         AdminImage='no'
                         Platform='x86'
                         ReadOnly='yes'
                         ShortNames='no'
                         InstallScope='perMachine'
                         Keywords='Installer,MSI,Database' />

                <MediaTemplate EmbedCab="yes" />

                <Upgrade Id="{$UpgradeCode}">
                    <UpgradeVersion Minimum="1.0.0.0"
                                    OnlyDetect="no"
                                    IncludeMinimum="no"
                                    Maximum="1.1.999.7100"
                                    IncludeMaximum="yes"
                                    Language="1033"
                                    Property="PREVIOUSVERSIONSINSTALLED" />
                </Upgrade>

                <InstallExecuteSequence>
                    <RemoveExistingProducts Before='InstallInitialize' />
                </InstallExecuteSequence>

                <Feature Id="ServicesGroup" Title="Services" Level="1">
                    <Feature Id="{$Component1}" Title="{$Component1_Title}" Level="1">
                        <ComponentGroupRef Id="{$Component1}_ServiceComponents" />
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
                        <Directory Id="CompanyFolder" Name="{$CompanyName}">
                            <Directory Id="ProductFolder" Name="{$ProductName}">
                                <Directory Id="{$Component1}_Folder" Name="{$Component1}" />
                            </Directory>
                        </Directory>
                    </Directory>
                    <Directory Id="TempFolder"/>
                </Directory>
            </Fragment>

            <Fragment>
                <ComponentGroup Id="{$Component1}_ServiceComponents">
                    <Component Id="{$Component1}_ServiceComponent" Directory="{$Component1}_Folder" Guid="06a90268-f38f-4c44-8648-afc2f78cd034">
                        <xsl:call-template name="Component1_ReferencesTemplate" />
                        <ServiceInstall
                            Id="CollaborationServiceInstaller"
                            Type="ownProcess"
                            Vital="yes"
                            Name="{$Component1_ServiceName}"
                            DisplayName="{$Component1_ServiceName}"
                            Description="{$Component1_ServiceDescr}"
                            Start="auto"
                            Account="LocalSystem"
                            ErrorControl="normal"
                            Interactive="no"
                        >
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
                        <ServiceControl Id="{$Component1}StartService" Start="install" Stop="both" Remove="both" Name="{$Component1_ServiceName}" Wait="yes" />
                    </Component>
                </ComponentGroup>
            </Fragment>
        </Wix>
    </xsl:template>

    <xsl:template name="Component1_ReferencesTemplate" match="@*|node()">
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
