Данный проект построен с использованием технологии ASP.NET и SignalR. 

![PrintHubPreview](https://github.com/Zoron87/PrintHub/assets/29422098/2fba769d-6ef0-408d-b90e-81666da199a6)

При запуске проекта происходит считывание имеющихся принтеров на сервере печати CUPS и вывод их в список доступных принтеров. После выбора принтера, выбора файла (через кнопку Browse/Обзор) и нажатия кнопки печати "Send Print Job" уходит задача печати на сервер печати. 
Все это происходит с использованием технологии веб-сокетов (SignalR).
