// include Deployment Tools Foundation (WIX)
#I @"."
#r @"Microsoft.Deployment.WindowsInstaller.dll"

// include Fake libs
#I @"./bin/FAKE/tools/"
#r @"FakeLib.dll"
#r @"Microsoft.Deployment.WindowsInstaller"

open Fake
open ServiceControllerHelpers
open Microsoft.Deployment.WindowsInstaller

let service = "Elders.PushNotifications"
let timeout = System.TimeSpan.FromMinutes 2.

// Targets
Target "PreInstall" (fun _ ->
    stopService service
    ensureServiceHasStopped service timeout
)

Target "Install" (fun _ ->

    let msi = "PushNotifications.WS.MSI.msi"

    let tova_e_taka_narochno = InstallUIOptions.Silent |> Installer.SetInternalUI
    Installer.EnableLog(InstallLogModes.Verbose, "InstallLog.txt");
    Installer.InstallProduct(msi,"")
)
Target "PostInstall" (fun _ ->
    
    while checkServiceExists service <> true do
        System.Threading.Thread.Sleep 500
        
    startService service
    ensureServiceHasStarted service timeout
)

"PreInstall"
  ==> "Install"
  ==> "PostInstall"
 
// start build
Run "PostInstall"