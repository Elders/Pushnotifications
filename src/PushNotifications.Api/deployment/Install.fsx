// include Fake libs
#I @"bin/FAKE/tools/"
#r @"FakeLib.dll"
#r @"Fake.IIS.dll"
#r @"Fake.Deploy.Lib.dll"
#r @"Fake.Deploy.exe"

open Fake
open Fake.IISHelper
open Fake.Services

let env = environVar "CLUSTER_NAME"
let defaultWebsite = "pushnotifications.local.com"
let website = getBuildParamOrDefault "website" defaultWebsite

let depl_path = environVar "vapt_depl_path"
let websiteDeplPath = depl_path @@ @"Elders\PushNotifications\pn.Api"

// Targets
Target "PreInstall" (fun _ ->
    let stopWebsite = "stop site " + website
    AppCmd stopWebsite
)

Target "Install" (fun _ ->
    XCopy "../content" websiteDeplPath
)
Target "PostInstall" (fun _ ->
    let startWebsite = "start site " + website
    AppCmd startWebsite
)

"PreInstall"
  ==> "Install"
  ==> "PostInstall"
 
// start build
Run "PostInstall"