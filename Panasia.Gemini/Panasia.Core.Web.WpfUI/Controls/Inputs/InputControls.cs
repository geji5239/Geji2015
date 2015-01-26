using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Panasia.Core.Wpf;

namespace Panasia.Core.Web.Controls.Inputs
{
    [UIControl(InputControlSettings.GroupName, InputControlSettings.TextArea)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputTextArea:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.TextBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputTextBox:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.PasswordBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputPasswordBox:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.Hidden)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputHidden:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.CheckBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputCheckBox:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.NumericBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputNumericBox:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.DateBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputDateBox:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.DateTimeBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputDateTimeBox:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.FileBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputFileBox:Input
    {

    }
    [UIControl(InputControlSettings.GroupName, InputControlSettings.ImageBox)]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]   
    public class InputImageBox:Input
    {

    }
}
