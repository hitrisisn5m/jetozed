@echo off
color 0e
@echo ==================================
@echo ���ѣ����Ҽ����ļ����ù���Ա��ʽ�򿪡�
@echo ==================================
@echo Start Remove FastTunnel.Server

Net stop FastTunnel.Server
sc delete FastTunnel.Server
pause