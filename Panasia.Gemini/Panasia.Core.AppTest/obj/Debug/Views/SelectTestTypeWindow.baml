   M S B A M L   `   `   `  ����K  GPanasia.Core.App, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM8clr-namespace:Panasia.Core.App;assembly=Panasia.Core.AppPanasia.Core.App  5      G CPanasia.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA0clr-namespace:Panasia.Core;assembly=Panasia.CorePanasia.Core 5       Panasia.Core.AppTest.   (Panasia.Core.AppTest.SelectSqlItemWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   N JFirstFloor.ModernUI, Version=1.0.5.0, Culture=neutral, PublicKeyToken=null0mui&http://firstfloorsoftware.com/ModernUI  5      Bapp8clr-namespace:Panasia.Core.App;assembly=Panasia.Core.App  5      K GPanasia.Core.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!afhttp://www.panasia.cn/pf  5      9db0clr-namespace:Panasia.Core;assembly=Panasia.Core  5      k	 gTelerik.Windows.Controls.Input, Version=2014.1.224.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7j
 fTelerik.Windows.Controls.Data, Version=2014.1.224.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7e aTelerik.Windows.Controls, Version=2014.1.224.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7n jTelerik.Windows.Controls.GridView, Version=2014.1.224.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7a ]Telerik.Windows.Data, Version=2014.1.224.45, Culture=neutral, PublicKeyToken=5803cfa389c90ce7Btk1http://schemas.telerik.com/2008/xaml/presentation 	 
    5        � ShowInTaskbar  .   � Title$ 选择数据表��5
   	   $	��160��6   $	��300��6$    � WindowStartupLocation$ CenterScreen=�60   .��5      �  � 	Resources 5   
    �� MergedDictionaries �� Source  Panasia.Core.SqlData   sqlData�� 5       5      �� 5      $: 4/Panasia.Core.Wpf;component/Assets/CommonStyles.xaml?�6)   %   &	          5      6   5       � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6   �� 5      $��30 �6    � ColumnDefinitions 5   
   �� 5      $��20 �6   �� 5      $
��Auto �6   �� 5      $	��180 �6   .+�5!   
   �� $��模块��6   $��1��6     EditTextBlock#��[ 6.   �� 5"   
   -   5         ��Name$ modules��5"      $��2��6$   	 �� DisplayMemberPath$
	 Name��5$      
 �� SelectedValuePath$

 Name��64     EditComboBox#��[ 5%      ��5#      ��  �� Source# [   �� Path$ Current.Groups�5%      �� 5'   
   $��	数据表��6   $��1��6   $��1��6,   #��[ 6<   �� 5(   
   -   5        $ tables��5(      $��1��6#   $��2��60   $
	 Name��5*      $

 Name��64   #��[ 5+      ��5)      ��  �� ElementName$ modules��$ SelectedItem.Items�5+      �� 5-   
   $��命令��6   $��2��6   $��1��6+   #��[ 6;   �� 5.   
   -   5        $
 sqls��5.      $��2��6!   $��2��6.   $
	 Name��50      $

 Name��64   #��[ 51      ��5/      �� $ tables��$ SelectedItem.Items�51      �� 53   
   $��3��6   $��3��6"    �� Orientation$ 
Horizontal=�62   $��Right=�6K   .+�54      �� -   5         ��Name$ btnOK��54      $��确定��68   $��23��6E   $��75��6Q   �� 55      -   5        $ 	btnCancel��55      $��取消��6@   $��23��6M   $��75��6Y   56      57      58      