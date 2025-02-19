# CRUD de Tarefas em C# com .NET
Esse é um CRUD de **tarefas bem simples** que eu fiz usando C# com .NET, tentando seguir umas boas práticas de arquitetura. O projeto usa o Factory Method, que basicamente ajuda a criar os objetos(databases) de um jeito mais flexível e deixa o código mais organizado pra dar manutenção depois.

Pra deixar tudo mais limpo e escalável, eu usei classes abstratas, o que ajudou a separar bem as responsabilidades e não virar aquela bagunça com código espalhado. Isso também facilita bastante caso eu queira melhorar ou adicionar mais coisas no futuro.

## Desafio Resolvido: Pintura Dinâmica da Coluna Status
Uma das partes mais dificeis foi fazer a pintura dinâmica da coluna Status. Não dava pra usar as soluções normais porque só precisava mudar a cor de uma única aba sem acabar com o resto da tabela. No fim, tive que criar um método de pintura do zero, pra garantir que só a célula certa mudasse sem mexer em mais nada.


![image](https://github.com/user-attachments/assets/8de4f0a8-c887-41bf-a1e7-7c9ade4b0de9)
![image](https://github.com/user-attachments/assets/225da260-e5f8-4413-9353-e580246eb9ce)
![image](https://github.com/user-attachments/assets/0a93112a-5171-4fe6-a1ef-4eabc5740cf8)
![image](https://github.com/user-attachments/assets/ab87fc0c-7d96-41ea-83e9-69cd78035824)
![image](https://github.com/user-attachments/assets/f23abf43-39ca-4329-b083-4826f112f055)
