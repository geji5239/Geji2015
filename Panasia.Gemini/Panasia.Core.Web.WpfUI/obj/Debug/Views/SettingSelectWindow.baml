   M S B A M L   `   `   `  ����  Panasia.Core.Web.WpfUI*    $Panasia.Core.Web.SettingSelectWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   $	��300��5   !   $	��400��6.     � ShowInTaskbar  .   � WindowStartupLocation$ CenterScreen=�6P    � Title 6	   �� *Title+.��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $�� ��6   .��5      �� $��Disabled=�67   $
��Auto=�5      ��5      �� *
Settings+��5      �� 5      .R�5      � 5      ��5      �� 5      .R�5      ��  �� Orientation$ 
Horizontal=�6%   $	��150��6>   .+�5      �� $��Center=�6T    d� 	IsChecked 6'   �� *
IsSelected+ �� Mode$ TwoWay=�6T   �� 5      $��Center=�5   4   $��5,0,0,0q�6O   ��5   (   �� *SettingItem.Name+5   O   �� 5      $��Center=�5   4   $��5,0,0,0q�6O   ��5   (   �� *SettingItem.Value+ �� StringFormat$ [{0}]��5   O   5      5      5      5      �� 5    
   $ 
Horizontal=�6   $��Right=�6.   $��1��6J   $��30��6W   .+�5!      �� $��确定��6   $��75��6"   $��Center=�6-   $��23��6H   ��5"      �� *	OKCommand+��5#      ��  �� RelativeSource �� 	 �� Mode$	 FindAncestor=�
 �� AncestorType$
 WindowL��� 5$      -   5         ��Name$ 	btnCancel��5$      $��取消��6@   $��75��6M   $��5,0,0,0q�6X   $��Center=�6i   $��23��6�   5%      5&      5'      