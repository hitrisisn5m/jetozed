@echo off
color 0e
@echo ==================================
@echo ���ѣ����Ҽ����ļ����ù���Ա��ʽ�򿪡�
@echo ==================================
@echo Start Remove FastTunnel.Client

Net stop FastTunnel.Client
sc delete FastTunnel.Client
pause