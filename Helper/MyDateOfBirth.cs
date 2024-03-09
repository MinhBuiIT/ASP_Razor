using System.ComponentModel.DataAnnotations;

public class MyDateOfBirth:ValidationAttribute {
    public MyDateOfBirth() { 
        ErrorMessage = "Năm sinh không hợp lệ - không được quá năm 2010";
    }
    public override bool IsValid(object? value) {
        if(value == null) return false;
        DateTime dateTime = (DateTime)value;
        if(dateTime < new DateTime(2010,1,1)) return true;
        return false;
    }
}