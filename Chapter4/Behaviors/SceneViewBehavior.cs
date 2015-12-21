using Esri.ArcGISRuntime.Controls;
using GalaSoft.MvvmLight.Messaging; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; 
using System.Windows.Interactivity;

namespace Chapter4.Behaviors
{
    public class SceneViewBehavior : Behavior<SceneView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }
        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            SceneView sceneView = sender as SceneView;
            Messenger.Default.Send<SceneView>(sceneView);
        }
    }
}
