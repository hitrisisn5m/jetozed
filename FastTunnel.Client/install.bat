@echo off
color 0e
@echo ==================================
@echo ���ѣ����Ҽ����ļ����ù���Ա��ʽ�򿪡�
@echo ==================================
@echo Start Install FastTunnel.Client

sc create FastTunnel.Client binPath=%~dp0\FastTunnel.Client.exe start= auto 
sc description FastTunnel.Client "FastTunnel-��Դ������͸���񣬲ֿ��ַ��https://github.com/SpringHgui/FastTunnel star��Ŀ��֧������"
Net Start FastTunnel.Client
pause