﻿using Windows.UI.Xaml.Controls;

using ElmishProgram = Elmish.Uno.Samples.SubModelSelectedItem.Program;

namespace Elmish.Uno.Samples.SubModelSelectedItem
{
    public partial class SubModelSelectedItemPage : Page
    {
        public SubModelSelectedItemPage()
        {
            InitializeComponent();
            ViewModel.StartLoop(ElmishProgram.Config, this, Elmish.ProgramModule.run, ElmishProgram.Program);
        }
    }
}
