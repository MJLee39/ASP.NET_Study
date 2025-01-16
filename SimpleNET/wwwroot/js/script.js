// wwwroot/js/script.js

fetch('/api/sqltest')  // 서버 API 호출
    .then(response => {
        if (response.ok) {
            return response.json();  // JSON 응답을 처리
        } else {
            return response.text();  // 텍스트 응답을 처리
        }
    })
    .then(data => {
        document.getElementById("status").textContent = data;  // 서버에서 받은 데이터 표시
    })
    .catch(error => {
        console.error("Error details:", error);  // 자세한 오류 콘솔 출력
        document.getElementById("status").textContent = "SQL Server 연결 오류: " + error.message;  // 오류 메시지 표시
    });
