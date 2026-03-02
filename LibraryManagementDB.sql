-- 1. Bảng Vai trò (Roles)
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL -- Admin, Librarian, Staff
);

-- 2. Bảng Tài khoản (Accounts)
CREATE TABLE Accounts (
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(256) NOT NULL,
    RoleId INT FOREIGN KEY REFERENCES Roles(RoleId),
    IsActive BIT DEFAULT 1
);

-- 3. Bảng Nhân viên (Staff)
CREATE TABLE Staff (
    StaffId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(15),
    Email VARCHAR(100),
    AccountId INT UNIQUE FOREIGN KEY REFERENCES Accounts(AccountId)
);

-- 4. Bảng Độc giả (Readers)
CREATE TABLE Readers (
    ReaderId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    IdentifyCard VARCHAR(20) UNIQUE NOT NULL, -- CMND/CCCD
    Phone VARCHAR(15),
    Address NVARCHAR(200),
    ExpiryDate DATE NOT NULL
);

-- 5. Bảng Thể loại sách (Categories)
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

-- 6. Bảng Sách (Books)
CREATE TABLE Books (
    BookId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Author NVARCHAR(100) NOT NULL,
    Publisher NVARCHAR(100),
    PublishYear INT,
    Quantity INT DEFAULT 0,
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId)
);

-- 7. Bảng Phiếu mượn (BorrowRecords)
CREATE TABLE BorrowRecords (
    RecordId INT IDENTITY(1,1) PRIMARY KEY,
    ReaderId INT FOREIGN KEY REFERENCES Readers(ReaderId),
    StaffId INT FOREIGN KEY REFERENCES Staff(StaffId),
    BorrowDate DATE DEFAULT GETDATE(),
    DueDate DATE NOT NULL,
    Status NVARCHAR(50) DEFAULT N'Đang mượn' -- Đang mượn, Đã trả, Quá hạn
);

-- 8. Bảng Chi tiết phiếu mượn (BorrowDetails)
CREATE TABLE BorrowDetails (
    RecordId INT FOREIGN KEY REFERENCES BorrowRecords(RecordId),
    BookId INT FOREIGN KEY REFERENCES Books(BookId),
    ReturnDate DATE NULL,
    FineAmount DECIMAL(18,2) DEFAULT 0,
    Notes NVARCHAR(200),
    PRIMARY KEY (RecordId, BookId)
);

-- INSERT DỮ LIỆU MẪU
INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Librarian');
INSERT INTO Accounts (Username, PasswordHash, RoleId) VALUES ('admin', 'admin123', 1); -- (Thực tế sẽ hash pass ở code)