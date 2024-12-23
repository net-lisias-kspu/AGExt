﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AGExt /L Unleashed")]
[assembly: AssemblyDescription("Extended Action Groups for KSP")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ActionGroupsExtended.LegalMamboJambo.Company)]
[assembly: AssemblyProduct(ActionGroupsExtended.LegalMamboJambo.Product)]
[assembly: AssemblyCopyright(ActionGroupsExtended.LegalMamboJambo.Copyright)]
[assembly: AssemblyTrademark(ActionGroupsExtended.LegalMamboJambo.Trademark)]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ad4873ce-5c81-4e76-8f80-4682c80ec9ee")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(ActionGroupsExtended.Version.Number)]
[assembly: AssemblyFileVersion(ActionGroupsExtended.Version.Number)]
[assembly: KSPAssembly("AGExt", ActionGroupsExtended.Version.major, ActionGroupsExtended.Version.minor)]

[assembly: KSPAssemblyDependency("KSPe", 2, 4)]
[assembly: KSPAssemblyDependency("KSPe.UI", 2, 4)]
