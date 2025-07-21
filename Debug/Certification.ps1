Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
# Generate a self-signed certificate
$cert = New-SelfSignedCertificate -Subject "CN=rex, O=corp, C=rexstudios.com" -DnsName "www.rexstudios.com" -Type CodeSigning -KeyUsage DigitalSignature -CertStoreLocation Cert:\CurrentUser\My -FriendlyName "ContactInformation"
# Set a password for the private key (optional)
$pw = ConvertTo-SecureString -String "admin12345" -Force -AsPlainText
# Export the certificate as a PFX file
Export-PfxCertificate -Cert $cert -FilePath 'C:\Contact Information\ContactInformation\bin\Debug\certificate.pfx' -Password $pw
# Display a success message
Write-Host "Self-signed certificate exported as certificate.pfx."