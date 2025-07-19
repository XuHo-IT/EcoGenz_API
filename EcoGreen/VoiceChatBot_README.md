# Voice Chat Bot với Lịch sử Chat và Tư vấn Hoạt động Tình nguyện

## Tính năng mới

Voice Chat Bot đã được cập nhật để hỗ trợ:
1. **Lưu trữ lịch sử chat** - giúp Gemini AI có thể trả lời chính xác hơn dựa trên context của cuộc hội thoại
2. **Tư vấn hoạt động tình nguyện** - sử dụng prompt chuyên biệt để gợi ý hoạt động phù hợp từ danh sách có sẵn

## Các thay đổi chính

### 1. AIChatService được cập nhật
- Hỗ trợ cả Ollama và Gemini API
- Tự động lưu trữ lịch sử chat trong `ChatModel`
- Sử dụng conversation history để cải thiện chất lượng phản hồi

### 2. GeminiClient được mở rộng
- Thêm method `AskWithHistoryAsync()` để gửi toàn bộ lịch sử chat
- Hỗ trợ format conversation của Gemini API

### 3. VoiceChatBotController được cải thiện
- Sử dụng `AIChatService` thay vì `GeminiClient` trực tiếp
- Lưu trữ lịch sử chat trong memory (có thể mở rộng để lưu database)
- Thêm endpoints để quản lý lịch sử chat
- **Tích hợp tư vấn hoạt động tình nguyện** với prompt chuyên biệt
- Sử dụng `ICompanyFormService` để lấy danh sách hoạt động thực tế

## API Endpoints

### 1. Voice Chat
```
POST /api/VoiceChatBot/voice-chat
```
- Upload file audio
- Nhận phản hồi audio từ AI với context từ lịch sử chat

### 2. Xem lịch sử chat
```
GET /api/VoiceChatBot/history
```
- Trả về toàn bộ lịch sử chat hiện tại

### 3. Xóa lịch sử chat
```
POST /api/VoiceChatBot/clear-history
```
- Xóa toàn bộ lịch sử chat

### 4. Refresh System Prompt
```
POST /api/VoiceChatBot/refresh-system-prompt
```
- Cập nhật lại system prompt với danh sách hoạt động mới nhất

### 5. Debug Activities
```
GET /api/VoiceChatBot/debug-activities
```
- Xem dữ liệu thực tế từ service hoạt động

### 6. Test với Sample Data
```
POST /api/VoiceChatBot/test-with-sample-data
```
- Test với dữ liệu mẫu để kiểm tra prompt hoạt động

### 7. Test Concise Response
```
POST /api/VoiceChatBot/test-concise
```
- Test với prompt ngắn gọn để tránh trả lời lan man

## Cách hoạt động

1. **Khởi tạo**: System prompt được tạo một lần duy nhất với danh sách hoạt động
2. **Speech-to-Text**: Chuyển đổi audio thành text (hoặc nhận transcript trực tiếp)
3. **Context Enhancement**: Nếu là tin nhắn đầu tiên, cập nhật system prompt với context của user
4. **Chat với context**: Gửi tin nhắn người dùng + lịch sử chat + enhanced system prompt đến Gemini
5. **Lưu lịch sử**: Tự động lưu cả tin nhắn người dùng và phản hồi AI
6. **Trả về JSON**: Response chứa reply, conversation history và transcript

## Lưu ý

- **System prompt được tạo một lần duy nhất** khi khởi tạo controller, giúp duy trì context xuyên suốt cuộc hội thoại
- **Context Enhancement**: Tin nhắn đầu tiên của user được đưa vào system prompt để cải thiện độ chính xác
- **Prompt được tối ưu để trả lời ngắn gọn** (dưới 100 từ) và tập trung vào recommend hoạt động
- Lịch sử chat hiện tại được lưu trong memory, sẽ mất khi restart server
- Có thể mở rộng để lưu vào database theo user/session
- Gemini API có giới hạn về độ dài context, nên có thể cần truncate lịch sử cũ
- **Có thể refresh system prompt** để cập nhật danh sách hoạt động mới
- Có fallback mechanism nếu không lấy được danh sách hoạt động

## Cải tiến tương lai

1. Lưu lịch sử chat vào database
2. Hỗ trợ multiple users/sessions
3. Giới hạn độ dài lịch sử chat để tránh vượt quá token limit
4. Thêm tính năng export/import lịch sử chat
5. **Cải thiện prompt engineering** để tối ưu hóa việc gợi ý hoạt động
6. **Thêm filtering** theo location, date, category của hoạt động
7. **Cache danh sách hoạt động** để tăng performance 