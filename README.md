# PHẦN MỀM WCS CHO HỆ BĂNG TẢI CON LĂN FORTNA

![Warehouse Control Systems](/assets/Fortna.2K.png)

### NGUYÊN LÝ HOẠT ĐỘNG
  Hệ thống băng tải và Phần mềm WCS Intech tích hợp với Hệ thống WES Fortna (Warehouse Excution System) điều khiển Khay tới các Trạm. 
  Dựa vào việc đọc mã vạch được dán trên khay. WCS đọc và gửi barcode tới WES -> WES trả về id của trạm cần đến -> WCS điều khiển Khay tới trạm đó.
  Sau khi người vận hành tại mỗi trạm hoàn thành công việc, WES gửi command tới WCS -> WCS điều khiển khay chứa rời trạm và tiếp tục quá trình sản xuất.

### CÁC TÍNH NĂNG
- Phần mềm WCS tích hợp với Phần mềm WES qua kết nối TCP/IP được mô tả trong API SPEC 20241101.docx
- Xỷ lý các message gửi và nhận từ WES
- Kết nối với Bộ điều khiển PLC Mitsubishi sử dụng thư viện MX Component, đọc trạng thái và ghi lệnh điều khiển xuống băng chuyền.
- Đọc mã barcode dán trên khay sử dụng Đầu đọc Datalogic qua kết nối TCP/IP.
- Hiển thị trạng thái ON/OFF của Stopper, Cụm chuyển làn, cảm biến,...
- Điều khiển chạy/dừng hệ thống: Start, Stop, Reset.
- Mô phỏng chính xác vị trí của khay ở trên băng tải.

### HOẠT ĐỘNG HỆ THỐNG
https://github.com/user-attachments/assets/a9e44f49-fa06-497b-852d-9417154e1ecb
