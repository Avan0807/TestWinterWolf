# Unity Intern Test – Winter Wolf / IEC Games

Ứng viên: **Nguyen Tuan Anh Van**  
Vị trí: **Intern Unity Developer**

---

## Phần 1 – Logic Test
[📺 Video demo (Google Drive)](https://drive.google.com/file/d/1OtPzUJFxFSBwGbHE5NczHdfFEv5mbGYG/view?usp=sharing)

### Task 1 – Xóa đúng 1 ký tự để chuỗi nhỏ nhất theo từ điển
- **File:** `Task1.cs`:contentReference[oaicite:3]{index=3}  
- **Class/Hàm:** `public static string Task1(string S)`  
- **Ý tưởng:**  
  - Duyệt chuỗi, xóa ký tự đầu tiên sao cho `S[i] > S[i+1]`.  
  - Nếu không tìm thấy thì xóa ký tự cuối cùng.  
- **Độ phức tạp:** O(N) thời gian, O(1) bộ nhớ phụ.  
- **Ví dụ mong đợi:** `"acb" -> "ab"`, `"hot" -> "ho"`, `"codility" -> "cdility"`, `"aaaa" -> "aaa"`.

### Task 2 – Hai quân xe, tổng điểm lớn nhất
- **File:** `Task2.cs`:contentReference[oaicite:4]{index=4}  
- **Class/Hàm:** `public static int Task2(int[,] A)`  
- **Ý tưởng:**  
  - Tính toán 2 giá trị lớn nhất theo từng hàng hoặc cột.  
  - Khi chọn 2 hàng (hoặc 2 cột), nếu rơi vào cùng cột (hoặc hàng) thì thay bằng giá trị lớn thứ hai.  
- **Độ phức tạp:** O(N*M + N^2) hoặc tương đương, phù hợp với constraint đề.  
- **Kết quả:** trả về tổng điểm lớn nhất khi đặt 2 xe không tấn công nhau.

### Task 3 – Biến mảng thành pairwise distinct với phép ±1
- **File:** `Task3.cs`:contentReference[oaicite:5]{index=5}  
- **Class/Hàm:** `public static int Task3(int[] A)`  
- **Ý tưởng:**  
  - Sort mảng.  
  - Ghép từng phần tử về dãy 1..N, tính tổng bước di chuyển |A[i] – (i+1)|.  
  - Nếu tổng bước > 1e9 thì trả về -1.  
- **Độ phức tạp:** O(N log N).

---

## Phần 2 – Unity Match-3 Test

### Task 1 – Re-skin
- Thay toàn bộ item trong Match-3 sang bộ hình cá (Fish).

### Task 2 – Đổi gameplay (5 ô đáy)
- Click vào 1 ô trên board → item rơi xuống 5 ô đáy.  
- Nếu có đúng 3 item giống nhau ở đáy → clear (scale to 0 và xóa).  
- **Win:** khi board trống và đáy rỗng.  
- **Lose:** khi 5 ô đáy đầy.  
- Thêm 2 chế độ test:  
  - **AutoPlay**: tự động chọn item theo nhóm nhiều nhất để win.  
  - **AutoLose**: cố tình lấp đầy 5 ô đáy để thua.

### Task 3 – Improve & Time Attack
- Đảm bảo board ban đầu có đủ loại cá.  
- Thêm animation khi di chuyển xuống đáy và khi clear.  
- Thêm chế độ **Time Attack (60s)**:  
  - Có nút **TimeAttack** trên HUD.  
  - Cho phép trả item từ đáy về lại board.  
  - Thua khi hết giờ chưa dọn xong.

---

## Scene & Scripts Unity

- **Scene:** `Scenes/Game.unity`  
- **Panels:** `PanelHome`, `PanelGame`, `PanelPause`, `PanelGameOver`, `PanelWin`  
- **Scripts chính:**  
  - Quản lý: `GameManager.cs`, `UIMainManager.cs`  
  - UI: `UIPanelMain.cs`, `UIPanelGame.cs`, `UIPanelGameOver.cs`  
  - Gameplay: `BoardController.cs`, `BottomMatchController.cs` 

---

## Cách chạy

1. Mở project bằng **Unity 2020.3.38f1**.  
2. Vào scene `Game` → bấm Play.  
3. Ở **PanelHome**: chọn chế độ Play.  
4. Trong **PanelGame**: có các nút **Pause**, **AutoPlay**, **AutoLose**, **TimeAttack**.  
5. Kết thúc → `PanelGameOver` hiển thị Win hoặc Lose.

---

**Nguyen Tuan Anh Van**  
Unity Intern Test – 2025
