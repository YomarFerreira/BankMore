# BankMore
┌────────────────────────────────────────┐

     ContaCorrente.API (porta 5001) \
       - Cadastro de contas \
       - Login (gera JWT) \
       - Movimentações (débito/crédito) \
       - Consulta de saldo 

└────────────────────────────────────────┘


┌────────────────────────────────────────┐

      ↓ (HTTP + JWT)

      Transferencia.API (porta 5002) \
       - Valida token JWT \
       - Chama ContaCorrente via HTTP \
       - Registra transferência \
       - Publica evento Kafka (opcional) 

└────────────────────────────────────────┘


┌────────────────────────────────────────┐

      ↓ (Kafka*)

     Tarifa.API (porta 5003) \
       - Consome eventos Kafka \
       - Calcula tarifas

└────────────────────────────────────────┘
 * para teste local - timeout de 3s


<img width="1332" height="917" alt="1" src="https://github.com/user-attachments/assets/c43afde9-4690-4738-b8e5-979ac2a2904b" />
<img width="1316" height="885" alt="2" src="https://github.com/user-attachments/assets/704a3c20-783e-4dab-96ff-c1d789cd212c" />
<img width="1327" height="917" alt="3" src="https://github.com/user-attachments/assets/3f603dfc-dba6-4d1b-a13d-6d7cd9359b52" />
<img width="1177" height="867" alt="4" src="https://github.com/user-attachments/assets/6076309e-9df2-4c4b-8e51-76737a360fc3" />
<img width="1170" height="920" alt="5" src="https://github.com/user-attachments/assets/b434566f-14cb-4ef9-bf15-032294c20248" />
<img width="1329" height="904" alt="6" src="https://github.com/user-attachments/assets/0ff82a5d-8695-4f59-b002-1b1bf8628c26" />
<img width="1320" height="596" alt="7" src="https://github.com/user-attachments/assets/b5b39ed1-e24c-4572-94db-2d16086bd5ee" />
<img width="1318" height="898" alt="8" src="https://github.com/user-attachments/assets/e6333d23-8df0-40f6-985a-37a22eef3ab5" />
