   M S B A M L   `   `   `  ����D  @Chapter2b, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Chapter2b.ViewModelsChapter2b.ViewModels  5   	       Chapter2b.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Z VEsri.ArcGISRuntime, Version=10.2.7.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad866esri+http://schemas.esri.com/arcgis/runtime/2013  5   	   :d2http://schemas.microsoft.com/expression/blend/2008  5   	   0locator"clr-namespace:Chapter2b.ViewModels  5   	   $	��600��5      $	��800��5	      .��5      �   � DataContext  6   ��    Locator �� Source# [   �� Path$ MainViewModel� � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   )  #Esri.ArcGISRuntime.Controls.MapView  -   5          Name$ 	MyMapView��5      $��0��6*   
   Map. 5      %  Esri.ArcGISRuntime.Controls.Map  -   5        
theMap��   Layers. 5      :  4Esri.ArcGISRuntime.Layers.ArcGISTiledMapServiceLayer  %  Esri.ArcGISRuntime.Layers.Layer	   ID$ Basemap��62      
ServiceUri 5      �� *MainViewModel.BasemapUri+# [  <  6Esri.ArcGISRuntime.Layers.ArcGISDynamicMapServiceLayer  5      $	 USA��64   	   
ServiceUri	 5      �� *MainViewModel.USAUri+# [  5      5      �� 5   
   -   5        $��Search��5      ���	#77000000$��Center=�5      $	��Top=�5      
 �� Padding$
 5q�6,   
��������� 5      .�6   
Search for6     �� 5      -   5        $��SearchTextBox��5      ��6+   �� *
SearchText+6I    �� 5      .�6   *(in the Cities, Counties or States layer.6?    �� 5       $
��Find��6   $��30��6$   ��6/   �� *SearchRelayCommand+��5!      �� $ SpatialReference.Wkid� �� ElementName$ 	MyMapView��5"      5#      &   System.Windows.Controls.DataGrid  5%   
   -   5        $��
MyDataGrid��5%         AutoGenerateColumns . $��2��5'      $	��200��6    ��5%   &   �� $ GridDataResults� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5'   0   5(      5)      