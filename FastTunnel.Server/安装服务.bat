@echo off
color 0e
@echo ==================================
@echo ���ѣ����Ҽ����ļ����ù���Ա��ʽ�򿪡�
@echo ==================================
@echo Start Install FastTunnel.Server

sc create FastTunnel.Server binPath=%~dp0\FastTunnel.Server.exe start= auto 
sc description FastTunnel.Server "FastTunnel-��Դ������͸���񣬲ֿ��ַ��https://github.com/SpringHgui/FastTunnel star��Ŀ��֧������"
Net Start FastTunnel.Server
pause