namespace DOAN.wwwroot.js
{
    public class dify_chatbot
    {
        document.addEventListener('DOMContentLoaded', function() {
            const userId = document.getElementById('chatbot-config')?.dataset?.userId;

            window.difyChatbotConfig = {
                token: 'iWjnpYz1rahLCVzv',
                systemVariables: {
                    user_id: userId
                },
            };
        });
    }
}
