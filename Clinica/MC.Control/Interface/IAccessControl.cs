using MC.Control.DTO;

namespace MC.Control.Interface
{
    public interface IAccessControl
    {
        (AccessDto, string) GenerateToken(AccessDto usuariodto);
    }
}
