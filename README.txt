Overview.

This is a prototype/proof-of-concept implementation of the end-to-end encryption feature, targeting requirements discussed internally.
* employs combination of symmetric and asymmetric key encryption
* creates a pair [encrypted file - encrypted key] for each subject file
* plugin architecture

A command line application serves as the host, instead of master control. Just for demo purposes.

0. To encrypt and decrypt files using this solution
* Compile solution
* Open command line at output directory of MCE2E.Cli. You'll find e2e.exe
* >e2e will print usage
* >e2e -help encrypt, provides usage details
* >e2e -help decrypt, provides usage details
* generate your own pub-priv key pairs from 1a.b.c below, or use the keys saved in the solution
* Set MCE2E.Controller.Configuration.PathToPublicKey to point to your public key file
* Ensure openssl.exe is available at location indicated by MCE2E.Controller.Configuration.PathToOpenSsl

Example, encrypting files in a given folder:
>e2e encrypt -targetdir c:\users\theUser\desktop\sandbox -ext *.txt
for each file xx in parent folder (sandbox), creates encrypted/ff.enc and encrypted/ff.key

Example, decrypting a file:
>e2e decrypt -targetdir c:\users\theUser\desktop\sandbox\encrypted -pkf c:\users\theUser\desktop\sandbox\privatekey.xml
for each file pair (ff.enc & ff.key), creates original ff


1. The user creates a public-private-key pair using the RSA algorithm (for example at Bosch a key length of 2048 bit is needed).
Creating a public-private key using openssl (openssl is installed with git, usually at C:\Program Files\Git\usr\bin).
* Generating pub-priv key-pair is not necessarily done using openssl
* Generating pub-priv key-pair is done by user, once
* Must be converted to xml-format so can be used by dotnet application
From https://gist.github.com/thinkerbot/706137, generate keys
a: openssl genrsa -out privatekey.pem
b: openssl rsa -in privatekey.pem -out publickey.pub -pubout
c: Transform both the public and private key into xml formats so key-pair can be used in dotnet. Use either online key-converter at https://superdry.apphb.com/tools/online-rsa-key-converter
or automate it using http://www.bouncycastle.org/csharp/

2. Once encrypted data has to be sent to the cloud, the ES820 creates a new AES key with key length of 256 bit, which is only valid 
 for this session.
The random symmetric key is also created using openssl. This is because of its inbuilt PRNG. (see MCE2E.Controller.Factories.OpenSSLKeyFactory.cs)
* DRVR will need openssl to be installed only for symmetric key generation
* I am sure with some effort we can find alternative means to generate a random key, thus removing the dependency on openssl.

3. E2E encryption logic, most closely matching Proposal1. With small adjustments, will also cater for Proposal2.

Create an AES key => AESKey //generates symmetric key
Data * AESKey => Data.Enc //encrypt data using unencrypyted symmetric key
public_RSA * AESKey => AESKey.Enc //encrypts symmetric key, store to local disk
Delete AESKey

Data can only be decrypted using AESKey, which is not available directly.
 It can only be obtained by having the private_RSA key, and applied to AESKey.Enc
private_RSA / AESKey.Enc => AESKey
Data.Enc / AESKey => Data 

4. Plugin architecture
The E2E encryption logic can be found in MCE2E.Controller.Services.DefaultEncryptionService.cs
This is the service which would be used by MC to encrypt data files.

The encryption functions (using AES) are defined in the library MCE2E.DefaultEncryption
These functions are:
* byte[] EncryptSymmetricKey(byte[] key)
* byte[] DecryptSymmetricKey(byte[] encryptedKey, FileInfo privateKeyFile)
* FileInfo Encrypt(byte[] key, FileInfo fileToEncrypt)
* FileInfo Decrypt(FileInfo encryptedFile, byte[] key)

An alternative encryption algorithm may be used and implemented in another library and plugged in to the application.
In this case, the plugin will be placed in arbitrary directory X, and the absolute path to this directory should be
passed to the Initialize(..) function in [de|en]cryption services at MCE2E.Controller.Services

4.1 Solution structure
MCE2E.Controller contains the E2E logic.
MCE2E.Contracts contains interfaces which would be needed by plugin developers.
MCE2E.DefaultEncryption is our default implementation using AES. It can be completely replaced by a 3rd party plugin.
MCE2E.Cli generates e2e.exe, which is a command line application to demonstrate encryption and decryption.

4.2 Plugin developers would have to 
a.Add a reference MCE2E.Contracts and implement
b. MCE2E.Contracts.IBootstrapper
c. MCE2E.Contracts.IEncryptionAlgorithm

