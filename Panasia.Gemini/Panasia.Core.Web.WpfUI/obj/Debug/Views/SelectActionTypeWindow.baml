   M S B A M L   `   `   `  ����E  AAF.AppCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=-clr-namespace:AF.Core.App;assembly=AF.AppCoreAF.Core.App  5      B >AF.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null2&clr-namespace:AF.Core;assembly=AF.CoreAF.Core 5       Panasia.Core.Web.WpfUI-   'Panasia.Core.Web.SelectActionTypeWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6mui&http://firstfloorsoftware.com/ModernUI     5      =app-clr-namespace:AF.Core.App;assembly=AF.AppCore     5      &afhttp://www.abiho.net/af     5      5db&clr-namespace:AF.Core;assembly=AF.Core     5      n jTelerik.Windows.Controls.GridView, Version=2014.1.224.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7:tk1http://schemas.telerik.com/2008/xaml/presentation  5        � ShowInTaskbar  .   � Title$ 选择功能��5
   	   $
��Auto��6   $	��360��6$    � WindowStartupLocation$ CenterScreen=�60    � SizeToContent$ Height=�6U   .��5      �  � 	Resources 5   
    �� MergedDictionaries �� Source�� 5       5      �� 5      $0 */AF.Wpf;component/Assets/CommonStyles.xaml?�6)   %    5       � RowDefinitions 5   
   �� 5      $
��Auto �6   $��90��6*   �� 5      $��30 �6   .+�5   
   �� $��功能类型��6   .��5   
   �� -   5         ��Name$ actions��5      $��2��6A   $��Disabled=�5      $
��Auto=�5      ��5   #   �� *Items+��5      �� 5      .R�5      � 	 � 	ItemWidth$		 100��6    5       ��5#      �� 5$      .R�5%      �� 
 �� Orientation$
 
Horizontal=�6%   $��30��6>   .+�5&      �� $��Center=�6'    d� 	IsChecked 6B   �� *
IsSelected+ �� Mode$ TwoWay=���6o   �� *
ActionName+5'      5(      5*      5+      �� 5-   
   $��1��6   $��2��6"   $
 
Horizontal=�62   $��Right=�6K   .+�5.      �� $��全选��6   $��Center=�6R   $��0,0,5,0q�6m    6$   �� *SelectedAll+$ TwoWay=�6m   �� 5/      -   5         ��Name$ btnOK��5/      $��确定��68   $��23��6E   $��75��6Q   �� 50      -   5        $ 	btnCancel��50      $��取消��6@   $��23��6M   $��75��6Y   51      52      53      