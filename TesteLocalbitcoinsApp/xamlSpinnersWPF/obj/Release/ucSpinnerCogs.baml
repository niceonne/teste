   M S B A M L   `   `   `  ����   xamlSpinnersWPF#    xamlSpinnersWPF.ucSpinnerCogs   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     >�Name$  UserControl�� � 	Resources  ��Name   spinner g� RepeatBehavior g� 	BeginTime �� 
TargetName �� TargetProperty9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value��%�  &	        �� $ spinner��$ Forever��.��U� $ 00:00:00X�$ 	bottomCog��$V P(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)�.\�  $ 00:00:00��$ 15Q�  $ 00:00:00.5000000��$	 195Q�  $ 00:00:01��$	 374Q�U� $ 00:00:00X�$ topCog��$V P(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)�.\�  $ 00:00:00��$ 0Q�  $ 00:00:00.5000000��$
 -180Q�  $ 00:00:01��$
 -359Q�	 � Triggers	 :� 
 :� RoutedEvent$
 FrameworkElement.Loaded��.[��� #��[  .���� -    ��Name$ uc_SpinnerCogs��$��24��$��Center=�$��Center=�$��24��.+�F� -    F�Name$ topCog�������4���_�@�V� 4���_�@UO�}?"@�]��3#@
|����@�� 
|����@S�
|����@o�FUO�}?"@o�F�V� �o�F�t�:�?�� :�?��]��3#@:�?4���_�@�t4���_�@�V� �71��V� �71�hR� 'R�B܋�� @�p&�ca�@Q����-���@���}    ј`GH'6 Sg.'���M�?㥛� �0@/"�/��?�� /"�/��?S�8� /"�/��?�`� @(u ҃����@��@��-���@    Q    �-���@��66 �_` ^Q���v@�P&A� @� X�� � �V� Ӡ OX� x�����@O��n= Q@�    ��}��-���@����Ζb�U�@��h�HJ_�!@�
}��� 	��� 	�����x�f0@	� O�|Ȕ=��g���}���������}���h��>Ӏ�X�71���G�C+0@71��V� 	
������� �� Stretch$
 Fill=�$��10��$��10�� � UseLayoutRounding .   �� Left$	 2.5��
 �� Top$	 2.5�� J� RenderTransformOrigin$ 0.5,0.5&�z�V� .~��� �� �� T� F� -   $ 	bottomCog�������4���_�@�V� 4���_�@UO�}?"@�]��3#@
|����@�� 
|����@S�
|����@o�FUO�}?"@o�F�V� �o�F�t�:�?�� :�?��]��3#@:�?4���_�@�t4���_�@�V� �71��V� �71�hR� 'R�B܋�� @�p&�ca�@Q����-���@���}    ј`GH'6 Sg.'���M�?㥛� �0@/"�/��?�� /"�/��?S�8� /"�/��?�`� @(u ҃����@��@��-���@    Q    �-���@��66 �_` ^Q���v@�P&A� @� X�� � �V� Ӡ OX� x�����@O��n= Q@�    ��}��-���@����Ζb�U�@��h�HJ_�!@�
}��� 	��� 	�����x�f0@	� O�|Ȕ=��g���}���������}���h��>Ӏ�X�71���G�C+0@71��V� 	
�������$
 Fill=�$��10��$��10�� .  $ 11��$ 10.25��$ 0.5,0.5&�z�V� .~��� �� ��  �� Angle$ 15Q�T� 