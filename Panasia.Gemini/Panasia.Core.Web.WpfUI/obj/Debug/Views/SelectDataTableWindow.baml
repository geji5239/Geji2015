   M S B A M L   `   `   `  ����E  AAF.AppCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=-clr-namespace:AF.Core.App;assembly=AF.AppCoreAF.Core.App  5      B >AF.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null2&clr-namespace:AF.Core;assembly=AF.CoreAF.Core 5       Panasia.Core.Web.WpfUI,   &Panasia.Core.Web.SelectDataTableWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6mui&http://firstfloorsoftware.com/ModernUI     5      =app-clr-namespace:AF.Core.App;assembly=AF.AppCore     5      &afhttp://www.abiho.net/af     5      5db&clr-namespace:AF.Core;assembly=AF.Core     5      n jTelerik.Windows.Controls.GridView, Version=2014.1.224.40, Culture=neutral, PublicKeyToken=5803cfa389c90ce7:tk1http://schemas.telerik.com/2008/xaml/presentation  5        � ShowInTaskbar  .   � Title$ 选择数据表��5
   	   $	��130��6   $	��300��6$    � WindowStartupLocation$ CenterScreen=�60   .��5      �  � 	Resources 5   
    �� MergedDictionaries �� Source  AF.Core.SqlData   sqlData�� 5       5      �� 5      $0 */AF.Wpf;component/Assets/CommonStyles.xaml?�6)   %   &	          5      6   5       � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6    � ColumnDefinitions 5   
   �� 5      $��20 �6   �� 5      $
��Auto �6   �� 5      $	��180 �6   .+�5    
   �� $��模块��6   $��1��6     EditTextBlock#��[ 6.   �� 5!   
   -   5         ��Name$ modules��5!      $��2��6$   	 �� DisplayMemberPath$
	 Name��5#      
 �� SelectedValuePath$

 Name��64     EditComboBox#��[ 5$      ��5"      ��  �� Source# [   �� Path$ Current.Groups�5$      �� 5&   
   $��	数据表��6   $��1��6   $��1��6,   #��[ 6<   �� 5'   
   -   5        $ tables��5'      $��1��6#   $��2��60   $
	 Name��5)      $

 Name��64   #��[ 5*      ��5(      ��  �� ElementName$ modules��$ SelectedItem.Items�5*      �� 5,   
   $��2��6   $��3��6"    �� Orientation$ 
Horizontal=�62   $��Right=�6K   .+�5-      �� -   5         ��Name$ btnOK��5-      $��确定��68   $��23��6E   $��75��6Q   �� 5.      -   5        $ 	btnCancel��5.      $��取消��6@   $��23��6M   $��75��6Y   5/      50      51      