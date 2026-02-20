using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarvaterWeb.Data
{
    public record IncomeDocumentDetails
    (
        string DocumentType,
        DateTime PlanningResponseDate,
        string Summary,
       // string Project,
        string SenderNumber,
        string Sender,
        DateTime FromDate
    );

    public record OutcomeDocumentDetails
        (
        string Summary,
        string Resipient,
        string Performer
        );

    public record ContractDetails
        (
        string ContractSubject,
        string ContractType,
        string Party1,
        string Party2,
        string Party1Name,
        string Party2Name,
        string Cost,
        string WithNDS,
        string TotalCost
        );

    public record NoteDetails
        (
        string DocumentType,
        string Topic,
        string Content,
        //string Adressees,
        string LastName,
        string FirstName
        );

    public record OrderDetails
        (
        string DocumentType,
        string OrderName,
        string OrderContent,
        //string Adressees,
        string LastName,
        string FirstName
        //DateTime SigningDate
        );

    public record GroupDetails
        (
        string GroupName,
        string Responsible,
        bool IsAdmin,
        bool IsGip,
        bool IsArchive,
        bool IsContracts,
        bool IsOrd        
        );

    public record DepartmentDetails
        (
        string Name,
        string Code
        );

    public record UserDetails(
        string LastName,
        string FirstName,
        string IDnumber,
        string Department,
        string Position,
        bool IsLeader,
        bool HasARightToSign,
        bool IsDomainUser,
        string AuthenticationType,
        string Login,
        string Language,
        string Phone,
        string Email
        );

    public record LegalDetails
     (
         string Inn,
         string ShortName,
         string FullName,
         string Address,
         string Ogrn,
         string Kpp,
         string Phone,
         string Email
     );
     
}
