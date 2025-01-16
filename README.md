# ASP.NET 원데이 서비스

---

### **목표**

1. ASP.NET MVC의 기본 구조와 작동 원리 이해
2. 간단한 웹 애플리케이션 개발
3. 주요 기능 활용: 라우팅, 컨트롤러, 뷰, 모델, 데이터베이스 연동

---

### 개발 환경 준비
macOS에서 Docker SQL Server 컨테이너와 로컬 ASP.NET 어플리케이션 통신

1. vscode 설치
2. .NET SDK 설치
3. vscode에 c# dev kit 라이브러리 설치

---

### **서비스 주제:**

**"간단한 도서 대여 관리 시스템"**

- 사용자는 도서 목록을 조회하고, 대여 상태를 확인하거나 대여를 신청할 수 있음.
- 관리자는 도서를 추가, 수정, 삭제하고 대여 상태를 관리할 수 있음.

---

### 시스템 설계

- **프론트엔드**:
    - MVVM 패턴:
        - **Model**: 도서 데이터 및 대여 상태.
        - **ViewModel**: Kendo UI를 활용해 데이터 바인딩 및 이벤트 처리.
        - **View**: Jquery와 Kendo UI Component로 UI 구성.
    - **AJAX 요청**:
        - API와 통신하여 데이터 조회 및 전송.
- **백엔드**:
    - ASP.NET MVC 5:
        - **Controller**: API 요청 처리.
        - **Model**: Entity Framework를 사용해 데이터베이스와 연동.
        - **View**: JSON 데이터를 전달.
- **데이터베이스**:
    - 테이블 설계:
        - **Books**: 도서 정보 테이블.
        - **Rentals**: 대여 정보 테이블.
- **서버**:
    - Microsoft Azure의 Windows Server에서 호스팅.
    - IIS 설정으로 애플리케이션 배포.
- **형상관리**:
    - GitHub에 저장소 생성.
    - 주요 브랜치: main, develop.

---

## 데이터베이스 - sql server를 docker에서 실행해 azuer data studio로 관리

- Books 테이블

```sql
CREATE TABLE Books (BookID INT PRIMARY KEY IDENTITY(1,1),Title NVARCHAR(100),Author NVARCHAR(100),Status NVARCHAR(20));
```

- Rentals 테이블

```sql
CREATE TABLE Rentals (RentalID INT PRIMARY KEY IDENTITY(1,1),BookID INT FOREIGN KEY REFERENCES Books(BookID),RentedBy NVARCHAR(100),RentalDate DATETIME,ReturnDate DATETIME);
```

SQL Server Docker 컨테이너 실행

```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password11" \
-p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Azure Data Studio 다운로드 → **SQL Server에 접속하기위해**

접속 정보:

- **서버**: `localhost,1433`
- **사용자 이름**: `sa`
- **비밀번호**: Password11

→ macOS는 **ARM64** 아키텍처! 윈도우는 AMD64!

`mssql-tools`를 설치하는 과정에서 "Unable to locate package" 오류가 발생하는 이유는 설치 과정에서 ARM64 아키텍처와 관련된 문제일 가능성이 큽니다. Microsoft는 `mssql-tools`를 포함한 일부 패키지를 주로 x86_64(amd64) 아키텍처에 맞춰 제공하므로 ARM64 기반 컨테이너에서는 설치가 어려울 수 있습니다.

해결하기위해 → **다른 Docker 이미지 사용** 

The string did not match the expected pattern. 에러 발생

```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MyApplication;User Id=SA;Password=Password11;TrustServerCertificate=True;Encrypt=False;"
  },
  "AllowedHosts": "*"
}

docker exec -it sqlserver bash
export PATH=$PATH:/opt/mssql-tools/bin
sqlcmd -S localhost -U SA -P password
```

---

## 백엔드

- **Controller 개발**:
    - BooksController:
        - GetBooks(): 도서 목록 반환.
        - UpdateBookStatus(int bookId, string status): 도서 상태 업데이트.
    - RentalsController:
        - RentBook(int bookId, string rentedBy): 도서 대여 처리.
        - ReturnBook(int bookId): 도서 반납 처리.
- **Model 생성**:
    - Entity Framework를 사용해 데이터베이스 연동:
        
        ```csharp
        public class Book {
            public int BookID { get; set; }
            public string Title { get; set; }
            public string Author { get; set; }
            public string Status { get; set; }
        }
        
        public class Rental {
            public int RentalID { get; set; }
            public int BookID { get; set; }
            public string RentedBy { get; set; }
            public DateTime RentalDate { get; set; }
            public DateTime? ReturnDate { get; set; }
        }
        
        ```
        

---

## 프론트엔드 개발

- **Kendo UI와 MVVM 패턴 적용**:

```html
<div id="bookGrid"></div>
<script>
  $(document).ready(function() {
      $("#bookGrid").kendoGrid({
          dataSource: {
              transport: {
                  read: {
                      url: "/Books/GetBooks",
                      dataType: "json"
                  }
              }
          },
          pageable: true,
          columns: [
              { field: "Title", title: "Title" },
              { field: "Author", title: "Author" },
              { field: "Status", title: "Status" },
              { command: { text: "Rent", click: rentBook }, title: "Action" }
          ]
      });
  });

  function rentBook(e) {
      var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
      $.ajax({
          url: "/Rentals/RentBook",
          type: "POST",
          data: { bookId: dataItem.BookID, rentedBy: "User1" },
          success: function() {
              alert("Book rented successfully!");
              $("#bookGrid").data("kendoGrid").dataSource.read();
          }
      });
  }
</script>
```

---

### **서버 설정**

- **Azure에서 Windows Server 구성**:
    - IIS에 ASP.NET MVC 배포.
    - 데이터베이스 연결 문자열 설정.

---

## **테스트 및 배포**

- **GitHub 활용**:
    - 개발 브랜치에서 코드 작성 후 Pull Request로 검토.
    - CI/CD를 통해 자동 배포 설정.
- **Azure 테스트**:
    - 웹 애플리케이션의 동작 확인.
    - 데이터베이스 연결 테스트.
