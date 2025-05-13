// Получаем ID пользователя из глобальной переменной, переданной из Razor
const currentUserId = window.currentUserId;

// Настройка SignalR
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", { accessTokenFactory: () => "your-token" })
    .build();

connection.on("ReceiveMessage", (chatId, sender, message, time) => {
    const chatContent = document.querySelector('.chat-content');
    if (chatContent.dataset.chatId === chatId) {
        const messagesDiv = chatContent.querySelector('.space-y-2');
        messagesDiv.innerHTML += `<div class="bg-gray-600 p-2 rounded max-w-md">${message}</div>`;
        // Прокручиваем вниз, чтобы видеть новое сообщение
        messagesDiv.scrollTop = messagesDiv.scrollHeight;
    }
    // Обновляем список чатов, чтобы отобразить последнее сообщение
    showChats();
});

connection.on("ReceiveNotification", (sender, message, date) => {
    showNotification(sender, message, date);
});

connection.start().catch(err => console.error(err));

function toggleNotifications(btn) {
    const icon = btn.querySelector('svg');
    icon.classList.toggle('hidden');
    btn.querySelectorAll('svg')[1].classList.toggle('hidden');
}

function showUserInfo(name) {
    const modal = document.createElement('div');
    modal.id = 'userInfoModal';
    modal.className = 'fixed inset-0 flex items-center justify-center bg-black bg-opacity-50';
    modal.innerHTML = `
        <div class="bg-gray-800 p-6 rounded-lg shadow-lg">
            <h2 class="text-xl font-bold mb-4 text-white">Информация о пользователе</h2>
            <p class="text-white mb-4">Имя: ${name}</p>
            <p class="text-white mb-4">Статус: Онлайн</p>
            <button onclick="closeModal()" class="bg-red-600 hover:bg-red-700 text-white py-2 px-4 rounded">Закрыть</button>
        </div>
    `;
    document.body.appendChild(modal);
}

function showProfile() {
    fetch(`/api/user/${currentUserId}`)
        .then(response => response.json())
        .then(user => {
            const mainContent = document.querySelector('.main-content');
            mainContent.innerHTML = `
                <div class="flex-1 flex flex-col p-4 bg-black bg-opacity-30">
                    <h2 class="text-xl font-bold mb-4">Мой профиль</h2>
                    <div id="profileForm" class="space-y-4">
                        <div>
                            <label class="block text-sm font-medium">Имя</label>
                            <input id="profileName" type="text" value="${user.name}" class="w-full bg-gray-700 text-white p-2 rounded" />
                        </div>
                        <div>
                            <label class="block text-sm font-medium">Никнейм</label>
                            <input id="profileNickname" type="text" value="${user.nickname}" class="w-full bg-gray-700 text-white p-2 rounded" />
                        </div>
                        <div>
                            <label class="block text-sm font-medium">Номер телефона</label>
                            <input id="profilePhone" type="text" value="${user.phone}" class="w-full bg-gray-700 text-white p-2 rounded" />
                        </div>
                        <div>
                            <label class="block text-sm font-medium">Email</label>
                            <input id="profileEmail" type="email" value="${user.email}" class="w-full bg-gray-700 text-white p-2 rounded" />
                        </div>
                        <div>
                            <label class="block text-sm font-medium">Дата регистрации</label>
                            <input type="text" value="${user.registrationDate}" class="w-full bg-gray-500 text-white p-2 rounded" disabled />
                        </div>
                        <button onclick="saveProfile()" class="bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded">Сохранить</button>
                    </div>
                </div>
            `;
        });
}

function saveProfile() {
    const updatedUser = {
        id: currentUserId,
        name: document.getElementById('profileName').value,
        nickname: document.getElementById('profileNickname').value,
        phone: document.getElementById('profilePhone').value,
        email: document.getElementById('profileEmail').value,
        registrationDate: document.querySelector('#profileForm input[disabled]').value
    };
    fetch(`/api/user/${currentUserId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updatedUser)
    }).then(() => alert('Профиль успешно обновлен!'));
}

function showContacts() {
    const chatList = document.querySelector('.chat-list');
    chatList.innerHTML = `
        <h2 class="text-lg font-semibold mb-4">Контакты</h2>
        <div class="cursor-pointer p-2 rounded bg-black bg-opacity-30 hover:bg-black hover:bg-opacity-50" onclick="showContactOptions('Никнейм123')">
            <div class="font-semibold">Никнейм123</div>
        </div>
    `;
    const mainContent = document.querySelector('.main-content');
    mainContent.innerHTML = '';
}

function showContactOptions(name) {
    const modal = document.createElement('div');
    modal.id = 'contactModal';
    modal.className = 'fixed inset-0 flex items-center justify-center bg-black bg-opacity-50';
    modal.innerHTML = `
        <div class="bg-gray-800 p-6 rounded-lg shadow-lg">
            <h2 class="text-xl font-bold mb-4 text-white">${name}</h2>
            <div class="flex flex-col space-y-3">
                <button onclick="startChat('${name}')" class="bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded">Написать сообщение</button>
                <button onclick="showUserInfo('${name}')" class="bg-gray-600 hover:bg-gray-700 text-white py-2 px-4 rounded">Информация</button>
                <button onclick="closeModal()" class="bg-red-600 hover:bg-red-700 text-white py-2 px-4 rounded">Закрыть</button>
            </div>
        </div>
    `;
    document.body.appendChild(modal);
}

function closeModal() {
    const modal = document.getElementById('contactModal') || document.getElementById('userInfoModal') || document.getElementById('notificationModal');
    if (modal) {
        modal.remove();
    }
}

function showChats() {
    fetch(`/api/chat/user/${currentUserId}`)
        .then(response => response.json())
        .then(chats => {
            const chatList = document.querySelector('.chat-list');
            let chatListHTML = '<h2 class="text-lg font-semibold mb-4">Чаты</h2>';
            // Асинхронно загружаем информацию о пользователях для каждого чата
            Promise.all(chats.map(chat => {
                const otherUserId = chat.user1Id === currentUserId ? chat.user2Id : chat.user1Id;
                return fetch(`/api/user/${otherUserId}`)
                    .then(res => res.json())
                    .then(user => ({ chat, user }));
            })).then(chatData => {
                chatData.forEach(({ chat, user }) => {
                    const lastMessage = chat.messages[chat.messages.length - 1];
                    chatListHTML += `
                        <div class="cursor-pointer p-2 rounded bg-black bg-opacity-30 hover:bg-black hover:bg-opacity-50" onclick="openChat('${chat.id}', '${user.nickname}')">
                            <div class="font-semibold">${user.nickname}</div>
                            <div class="text-sm text-gray-400">${lastMessage ? lastMessage.content : 'Нет сообщений'}</div>
                            <div class="text-xs text-right text-gray-500">${lastMessage ? new Date(lastMessage.timestamp).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : ''}</div>
                        </div>
                    `;
                });
                chatList.innerHTML = chatListHTML;
            });
        });
}

function openChat(chatId, name) {
    fetch(`/api/chat/${chatId}`)
        .then(response => response.json())
        .then(chat => {
            const chatContent = document.querySelector('.chat-content');
            chatContent.dataset.chatId = chatId; // Обновляем текущий chatId
            let messagesHTML = '';
            chat.messages.forEach(msg => {
                messagesHTML += `<div class="bg-gray-600 p-2 rounded max-w-md">${msg.content}</div>`;
            });
            chatContent.innerHTML = `
                <div class="messages-container">
                    <h2 class="text-xl font-bold cursor-pointer" onclick="showUserInfo('${name}')">${name}</h2>
                    <div class="space-y-2 mt-4">${messagesHTML}</div>
                </div>
                <div class="flex mt-4">
                    <input type="text" placeholder="Введите сообщение..." class="w-3/4 bg-gray-700 text-white p-2 rounded-l" />
                    <button onclick="sendMessage('${chatId}')" class="bg-blue-600 hover:bg-blue-700 text-white p-2 rounded-r">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                        </svg>
                    </button>
                </div>
            `;
            // Прокручиваем вниз, чтобы видеть последние сообщения
            const messagesDiv = chatContent.querySelector('.space-y-2');
            messagesDiv.scrollTop = messagesDiv.scrollHeight;
        });
}

function sendMessage(chatId) {
    const input = document.querySelector('.chat-content input');
    if (input.value.trim()) {
        connection.invoke("SendMessage", chatId, currentUserId, input.value).catch(err => console.error(err));
        input.value = '';
    }
}

function startChat(name) {
    // Здесь нужно создать новый чат, если его нет
    // Для простоты просто обновляем список чатов
    showChats();
    closeModal();
}

function showNotification(sender, message, date) {
    const modal = document.createElement('div');
    modal.id = 'notificationModal';
    modal.className = 'fixed bottom-4 right-4 bg-gray-800 p-4 rounded-lg shadow-lg w-80';
    modal.innerHTML = `
        <div class="text-white">
            <h3 class="font-bold">${sender}</h3>
            <p>${message}</p>
            <p class="text-sm text-gray-400">${date}</p>
            <button onclick="closeModal()" class="mt-2 bg-red-600 hover:bg-red-700 text-white py-1 px-2 rounded">Закрыть</button>
        </div>
    `;
    document.body.appendChild(modal);
}

window.onload = () => {
    showChats();
};