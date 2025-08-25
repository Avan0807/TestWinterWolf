# Unity Intern Test – Match3 2025

## Thời gian làm
- Thực hiện trong < 4 giờ.

## Các phần đã hoàn thành
### Task 1
- Xây dựng hệ thống menu:
  - `PanelHome`, `PanelGame`, `PanelPause`, `PanelGameOver`, `PanelWin`.
- Quản lý state game qua `GameManager` + `UIMainManager`.

### Task 2
- Thêm gameplay mới: kéo item xuống **5 ô đáy**.
  - Clear khi có 3 item giống nhau.
  - Thắng khi board trống và đáy rỗng.
  - Thua khi 5 ô đáy đầy.
- Thêm chế độ test:
  - **AutoPlay** (tự chơi).
  - **AutoLose** (cố tình thua).

### Task 3
- Thêm **Time Attack mode** (60 giây).
  - Hiển thị timer.
  - Cho phép trả item từ đáy về board.
  - Xử lý Win/Lose theo thời gian.

## Scripts chỉnh sửa/thêm mới
- `GameManager.cs`
- `UIMainManager.cs`
- `UIPanelMain.cs`
- `UIPanelGame.cs`
- `UIPanelGameOver.cs`
- `BottomMatchController.cs`

## Cách chạy
1. Mở project bằng Unity 2020.3.38f1.
2. Chạy scene `Game`.
3. Bấm **Play** ở `PanelHome`.
4. Trong khi chơi (`PanelGame`) có các nút:
   - **AutoPlay**
   - **AutoLose**
   - **TimeAttack`
5. Kết thúc → hiện `PanelGameOver` báo Win hoặc Lose.

---

👨‍💻 Thực hiện: Unity Intern Test 2025
