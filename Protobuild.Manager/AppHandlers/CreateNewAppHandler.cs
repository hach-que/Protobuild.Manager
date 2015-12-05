﻿using System;
using System.Collections.Specialized;

namespace Protobuild.Manager
{
    public class CreateNewAppHandler : IAppHandler
    {
        private readonly IIDEControl _ideControl;

        public CreateNewAppHandler(IIDEControl ideControl)
        {
            _ideControl = ideControl;
        }

        public void Handle(NameValueCollection parameters)
        {
            Console.WriteLine("would move to create screen");
            
            //_ideControl.LoadSolution(@"C:\Users\June\Documents\Projects\MonoGame", "MonoGame.Framework", "WindowsUniversal");
        }
    }
}