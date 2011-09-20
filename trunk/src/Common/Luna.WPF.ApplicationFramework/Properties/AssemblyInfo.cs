using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Windows;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Luna.WPF.ApplicationFramework")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Luna.WPF.ApplicationFramework")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2009")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("554e7a42-6d81-4b45-8f79-8ecbfb394f05")]
[assembly: ThemeInfo(
ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page,
    // or application resource dictionaries)
ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page,
    // app, or any theme specific resource dictionaries)
)]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Interfaces")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Converters")]

[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Controls")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna/brick", "Luna.WPF.ApplicationFramework.Controls.Brick")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna/cell", "Luna.WPF.ApplicationFramework.Controls.Cell")]
//[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.ResourceManager")]
//x[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Validations")]
[assembly: XmlnsDefinition("http://www.grandsys.com/lunacontrols", "Luna.WPF.ApplicationFramework.Controls")]
//[assembly: XmlnsDefinition("http://www.grandsys.com/lunacontrols", "Luna.WPF.ApplicationFramework.ControlExtensions")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.DataTemplateSelectors")]
//[assembly: XmlnsDefinition("http://www.grandsys.com/luna/controls/blockmatrix", "Luna.WPF.ApplicationFramework.Controls.BlockMatrix")]
//[assembly: XmlnsDefinition("http://www.grandsys.com/luna/controls/newblockmatrix", "Luna.WPF.ApplicationFramework.Controls.NewBlockMatrix")]

[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Markup")]
//[assembly: XmlnsDefinition("http://www.grandsys.com/luna/controls/customwindow", "Luna.WPF.ApplicationFramework.Controls.CustomWindow")]
//[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Interactivity")]
//[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Graphics")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Behaviors")]
[assembly: XmlnsDefinition("http://www.grandsys.com/luna", "Luna.WPF.ApplicationFramework.Collections")]